namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 复制任务批处理器 —— 拉取 PENDING 记录并并发调度执行
/// </summary>
public interface IDuplicateBatchProcessor
{
    /// <summary>
    /// 执行待处理的复制记录（定时任务调用）
    /// 内部自行管理 scope 和 UOW 生命周期
    /// </summary>
    Task ExecutePendingAsync();

    /// <summary>
    /// 执行单条复制记录（RabbitMQ 消费者调用）
    /// 按 LogId 读取记录并调用对应的内部/外部复制服务
    /// </summary>
    Task ProcessSingleAsync(long logId);
}
