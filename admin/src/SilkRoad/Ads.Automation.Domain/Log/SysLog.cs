namespace Ads.Automation.Domain.Log;

/// <summary>
/// 系统日志实体
/// </summary>
public class SysLog : AggregateRootEntity, IHasCreationTimeEntity
{
    /// <summary>
    /// 日志级别（Information / Warning / Error / Critical / Debug / Trace）
    /// </summary>
    public string Level { get; private set; } = string.Empty;

    /// <summary>
    /// 日志分类名称（如 "Ads.Automation.Api.Middleware.ResponseWrapperMiddleware"）
    /// </summary>
    public string Logger { get; private set; } = string.Empty;

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// 异常信息（如有）
    /// </summary>
    public string? Exception { get; private set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string? RequestPath { get; private set; }

    public long CreatorId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;

    private SysLog() { }

    public static SysLog Create(
        long id,
        string level,
        string logger,
        string message,
        string? exception = null,
        string? requestPath = null)
    {
        return new SysLog
        {
            Id = id,
            Level = level,
            Logger = logger,
            Message = message,
            Exception = exception,
            RequestPath = requestPath,
            CreationTime = DateTime.Now
        };
    }
}
