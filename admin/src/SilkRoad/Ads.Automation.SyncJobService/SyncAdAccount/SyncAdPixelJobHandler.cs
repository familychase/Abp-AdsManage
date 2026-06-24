using Ads.Automation.Application.Entity.MetaOAuth;
using Ads.Automation.Domain.Pixel;
using Ads.Automation.Infrastructure.Caching.Interfaces;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// 广告像素同步 RabbitMQ 消费者
/// 按 AccountNo 拉取 Meta 像素列表 → upsert 到 ads_pixel + 维护 ads_account_pixel 关联
/// </summary>
public class SyncAdPixelJobHandler : JobHandlerBase<SyncAdPixelJobArgs>
{
    public SyncAdPixelJobHandler(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger<SyncAdPixelJobHandler> logger)
        : base(consumer, scopeFactory, distributedLock, logger)
    {
    }

    protected override TimeSpan LockExpiration => TimeSpan.FromMinutes(5);

    protected override string BuildLockKey(SyncAdPixelJobArgs args)
        => $"pixel_sync:{args.AccountNo}";

    protected override async Task ExecuteCoreAsync(IServiceProvider sp, SyncAdPixelJobArgs args, CancellationToken ct)
    {
        var uowManager = sp.GetRequiredService<IUnitOfWorkManager>();
        using var uow = uowManager.Begin(requiresNew: true);

        var pixelRepo = sp.GetRequiredService<IBaseRepository<AdsPixel>>();
        var channelAdAccountRepo = sp.GetRequiredService<IBaseRepository<AdsChannelAdAccount>>();
        var accountPixelRepo = sp.GetRequiredService<IBaseRepository<AdsAccountPixel>>();
        var asyncExecuter = sp.GetRequiredService<IAsyncQueryableExecuter>();
        var metaAuth = sp.GetRequiredService<IMetaAuthorizationGateway>();
        var retry = sp.GetRequiredService<MetaApiRetryPolicy>();
        var logger = sp.GetRequiredService<ILogger<SyncAdPixelJobHandler>>();

        // Step 1: 通过 AccountNo 查找 ChannelId
        var relationQuery = await channelAdAccountRepo.GetQueryableAsync();
        var relation = await asyncExecuter.FirstOrDefaultAsync(
            relationQuery.Where(r => r.AccountNo == args.AccountNo), ct);
        if (relation == null)
        {
            logger.LogWarning("账户 {AccountNo} 未找到关联渠道，跳过像素同步", args.AccountNo);
            await uow.CompleteAsync();
            return;
        }

        // Step 2: 获取 AccessIdentity
        var identity = await metaAuth.GetAccessIdentityAsync(relation.ChannelId);
        ct.ThrowIfCancellationRequested();

        logger.LogInformation("开始同步账户 {AccountNo} 的像素", args.AccountNo);

        var accountActNo = $"act_{args.AccountNo}";
        var pixelQuery = await pixelRepo.GetQueryableAsync();
        string? after = null;
        var pixelByNo = new Dictionary<string, AdsPixel>();
        var totalCount = 0;

        // Step 3: 游标分页循环拉取像素数据
        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var pixels = await retry.ExecuteAsync(
                () => MetaOpenApi.GetAdPixelsAsync(identity, accountActNo, 500, MetaConst.PixelFileds, after),
                "像素同步");

            if (pixels?.data == null || pixels.data.Count == 0)
                break;

            var pixelIds = pixels.data.Where(p => p != null).Select(p => p.id).Distinct().ToList();
            var existingPixels = await asyncExecuter.ToListAsync(
                pixelQuery.Where(e => pixelIds.Contains(e.PixelNo)));
            var pixelMap = existingPixels.ToDictionary(e => e.PixelNo);

            foreach (var metaPixel in pixels.data)
            {
                ct.ThrowIfCancellationRequested();

                if (pixelMap.TryGetValue(metaPixel.id, out var existing))
                {
                    existing.SetPixelName(metaPixel.name ?? string.Empty);
                    existing.SetCode(metaPixel.code);
                    existing.SetLastSyncTime(DateTime.Now);
                    await pixelRepo.UpdateAsync(existing);
                    pixelByNo[metaPixel.id] = existing;
                }
                else
                {
                    var pixel = AdsPixel.Create(metaPixel.id, metaPixel.name ?? string.Empty, metaPixel.code);
                    await pixelRepo.InsertAsync(pixel);
                    pixelByNo[metaPixel.id] = pixel;
                }

                totalCount++;
            }

            var next = pixels.paging?.cursors?.after;
            if (string.IsNullOrEmpty(next)) break;
            after = next;
        }

        // ===== 维护账户-像素关联关系 =====
        var accountPixelQuery = await accountPixelRepo.GetQueryableAsync();
        var existingRelations = await asyncExecuter.ToListAsync(
            accountPixelQuery.Where(r => r.AccountNo == args.AccountNo));
        var relationByPixelNo = existingRelations.ToDictionary(r => r.PixelNo);

        var syncedPixelNos = new HashSet<string>(pixelByNo.Keys);
        var insertedRelationCount = 0;

        foreach (var pixelNo in syncedPixelNos)
        {
            ct.ThrowIfCancellationRequested();
            if (!relationByPixelNo.ContainsKey(pixelNo))
            {
                await accountPixelRepo.InsertAsync(new AdsAccountPixel
                {
                    AccountNo = args.AccountNo,
                    PixelId = pixelByNo[pixelNo].Id,
                    PixelNo = pixelNo
                });
                insertedRelationCount++;
            }
        }

        var relationsToDelete = existingRelations
            .Where(r => !syncedPixelNos.Contains(r.PixelNo)).ToList();
        foreach (var r in relationsToDelete)
            await accountPixelRepo.DeleteAsync(r);

        await uow.CompleteAsync();

        logger.LogInformation(
            "账户 {AccountNo} 像素同步完成，像素 {PixelCount} 条，新增关联 {InsertedCount} 条，移除关联 {RemovedCount} 条",
            args.AccountNo, totalCount, insertedRelationCount, relationsToDelete.Count);
    }
}
