namespace Ads.Automation.Application.Entity.Duplicate;

/// <summary>
/// 复制任务批处理器
/// 每轮从 PENDING 队列取最多 <see cref="MaxConcurrent"/> 条，并发执行
/// 自行管理 scope 和 UOW，不依赖 ABP 自动 UOW 拦截器
/// </summary>
public class DuplicateBatchProcessor : ITransientDependency, IDuplicateBatchProcessor
{
    private const int MaxConcurrent = 200;

    private readonly IServiceScopeFactory _scopeFactory;

    public DuplicateBatchProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ExecutePendingAsync()
    {
        using var batchScope = _scopeFactory.CreateScope();
        var sp = batchScope.ServiceProvider;
        var uowMgr = sp.GetRequiredService<IUnitOfWorkManager>();
        var sf = sp.GetRequiredService<IServiceScopeFactory>();
        var errorParser = sp.GetRequiredService<MetaErrorParser>();
        var logger = sp.GetRequiredService<ILogger<DuplicateBatchProcessor>>();

        // 显式创建 UOW：DuplicateBatchProcessor 没有继承 ApplicationService，
        // 不会获得 ABP 自动 UOW 拦截器，必须手动管理 DbContext 生命周期
        using var uow = uowMgr.Begin(requiresNew: true, isTransactional: true);

        var repo = sp.GetRequiredService<IBaseRepository<AdsDuplicateLogging>>();

        var query = await repo.GetQueryableAsync();

        var inProgressCount = query.Count(l => l.State == DuplicateState.IN_PROGRESS);
        var availableSlots = MaxConcurrent - inProgressCount;
        if (availableSlots <= 0)
            return;

        var now = DateTime.Now;
        var pending = query
            .Where(l => l.State == DuplicateState.PENDING && l.ScheduleTime <= now)
            .Take(availableSlots)
            .ToList();

        if (pending.Count == 0)
            return;

        var pendingIds = pending.Select(l => l.Id).ToList();

        foreach (var log in pending)
        {
            log.SetState(DuplicateState.IN_PROGRESS);
            await repo.UpdateAsync(log);
        }

        // 显式提交，确保子 scope 能读到 IN_PROGRESS 状态
        await uow.CompleteAsync();

        // 并发执行，每条记录独立 scope
        await Parallel.ForEachAsync(pendingIds,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 },
            async (id, ct) =>
            {
                using var itemScope = sf.CreateScope();
                var itemSp = itemScope.ServiceProvider;
                var itemRepo = itemSp.GetRequiredService<IBaseRepository<AdsDuplicateLogging>>();
                var internalService = itemSp.GetRequiredService<MetaDuplicateService>();
                var externalService = itemSp.GetRequiredService<MetaDuplicateExternalService>();
                var itemErrorParser = itemSp.GetRequiredService<MetaErrorParser>();
                var itemLogger = itemSp.GetRequiredService<ILogger<DuplicateBatchProcessor>>();

                var log = await itemRepo.GetAsync(id);
                if (log == null)
                {
                    itemLogger.LogWarning("复制记录不存在，跳过：{LogId}", id);
                    return;
                }

                try
                {
                    if (log.IsInternal)
                        await internalService.ProcessInternalPendingAsync(log);
                    else
                        await externalService.ProcessExternalPendingAsync(log);

                    log.SetEndTime(DateTime.Now);
                }
                catch (Exception ex)
                {
                    var errorMsg = itemErrorParser.ExtractErrorMessage(ex);
                    itemLogger.LogError(ex, "广告复制任务失败 - {Error}", ex.Message);
                    RecordError(log, errorMsg);
                    log.SetEndTime(DateTime.Now);
                }

                await itemRepo.UpdateAsync(log);
            });
    }

    /// <inheritdoc />
    public async Task ProcessSingleAsync(long logId)
    {
        using var itemScope = _scopeFactory.CreateScope();
        var sp = itemScope.ServiceProvider;
        var repo = sp.GetRequiredService<IBaseRepository<AdsDuplicateLogging>>();
        var internalService = sp.GetRequiredService<MetaDuplicateService>();
        var externalService = sp.GetRequiredService<MetaDuplicateExternalService>();
        var errorParser = sp.GetRequiredService<MetaErrorParser>();
        var logger = sp.GetRequiredService<ILogger<DuplicateBatchProcessor>>();

        var log = await repo.GetAsync(logId);
        if (log == null)
        {
            logger.LogWarning("复制记录不存在，跳过：{LogId}", logId);
            return;
        }

        // 幂等性：跳过非 PENDING 状态（已被其他方式处理）
        if (log.State != DuplicateState.IN_PROGRESS)
        {
            logger.LogInformation("复制记录非 IN_PROGRESS 状态，跳过：{LogId}, State={State}", logId, log.State);
            return;
        }

        try
        {
            if (log.IsInternal)
                await internalService.ProcessInternalPendingAsync(log);
            else
                await externalService.ProcessExternalPendingAsync(log);

            log.SetEndTime(DateTime.Now);
        }
        catch (Exception ex)
        {
            var errorMsg = errorParser.ExtractErrorMessage(ex);
            logger.LogError(ex, "广告复制任务失败 - LogId:{LogId}, Error:{Error}", logId, ex.Message);
            RecordError(log, errorMsg);
            log.SetEndTime(DateTime.Now);
        }

        await repo.UpdateAsync(log);
    }

    private static void RecordError(AdsDuplicateLogging log, string errorMsg)
    {
        log.SetState(DuplicateState.FAILED);
        log.SetErrorMessage(errorMsg);
    }
}
