using Ads.Automation.Infrastructure.Caching.Interfaces;

namespace Ads.Automation.SyncJobService.SyncAdAccount;

/// <summary>
/// Job 消费者基类 —— 封装分布式锁、Scope 管理、异常捕获 → MQ 重试。
/// 子类只需覆写 <see cref="BuildLockKey"/> + <see cref="ExecuteCoreAsync"/>。
///
/// 钩子：
///   <see cref="BeforeLockAsync"/> — 锁获取前的预检查（默认继续），
///     覆写 return false 可跳过本次执行（如非活跃账户检查）
///
/// 用法：
/// <code>
/// public class MyHandler : JobHandlerBase&lt;MyArgs&gt;
/// {
///     protected override string BuildLockKey(MyArgs msg) => $"my:{msg.Id}";
///     protected override async Task ExecuteCoreAsync(IServiceProvider sp, MyArgs msg, CancellationToken ct) { ... }
/// }
/// </code>
/// </summary>
/// <typeparam name="TMessage">Job 消息类型</typeparam>
public abstract class JobHandlerBase<TMessage> : RabbitMqConsumerBase<TMessage>
{
    private readonly IDistributedLock _distributedLock;
    private readonly ILogger _logger;

    protected JobHandlerBase(
        IMessageConsumer consumer,
        IServiceScopeFactory scopeFactory,
        IDistributedLock distributedLock,
        ILogger logger)
        : base(consumer, logger)
    {
        ScopeFactory = scopeFactory;
        _distributedLock = distributedLock;
        _logger = logger;
    }

    /// <summary>Scope 工厂，子类可在 <see cref="BeforeLockAsync"/> 中创建独立 Scope</summary>
    protected IServiceScopeFactory ScopeFactory { get; }

    // ========== 被子类覆写的配置点 ==========

    /// <summary>锁过期时间，默认 10 分钟</summary>
    protected virtual TimeSpan LockExpiration => TimeSpan.FromMinutes(10);

    /// <summary>根据消息生成锁键（必须覆写）</summary>
    protected abstract string BuildLockKey(TMessage message);

    /// <summary>
    /// 获取锁前的预检查钩子。返回 false 跳过本次执行（ACK，不重试）。
    /// 默认什么都不检查直接继续。
    /// </summary>
    protected virtual Task<bool> BeforeLockAsync(TMessage message, CancellationToken ct)
        => Task.FromResult(true);

    /// <summary>核心业务逻辑（必须覆写）。IServiceProvider 为每次执行独立创建的 Scope。</summary>
    protected abstract Task ExecuteCoreAsync(IServiceProvider sp, TMessage message, CancellationToken ct);

    // ========== 模板方法（sealed，防覆写绕过） ==========

    /// <inheritdoc />
    protected sealed override async Task<bool> HandleAsync(TMessage message, CancellationToken ct)
    {
        // 1. 预检查
        if (!await BeforeLockAsync(message, ct).ConfigureAwait(false))
            return true;

        // 2. 获取分布式锁
        var lockKey = BuildLockKey(message);
        if (!await _distributedLock.AcquireAsync(lockKey, LockExpiration))
        {
            _logger.LogInformation("[{Handler}] 锁冲突跳过: LockKey={LockKey}", GetType().Name, lockKey);
            return true;
        }

        // 3. 执行 + 异常 → MQ 重试
        try
        {
            using var scope = ScopeFactory.CreateScope();
            await ExecuteCoreAsync(scope.ServiceProvider, message, ct).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "[{Handler}] 执行失败: LockKey={LockKey}", GetType().Name, lockKey);
            return false; // NACK → MQ 重试
        }
        finally
        {
            await _distributedLock.ReleaseAsync(lockKey);
        }
    }
}
