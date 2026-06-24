namespace Ads.Automation.Domain.Log;

/// <summary>
/// 警告日志实体
/// </summary>
public class SysLogWarning : AggregateRootEntity, IHasCreationTimeEntity
{
    public string Level { get; private set; } = string.Empty;
    public string Logger { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? Exception { get; private set; }
    public string? RequestPath { get; private set; }

    public long CreatorId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;

    private SysLogWarning() { }

    public static SysLogWarning Create(long id, string level, string logger, string message, string? exception = null, string? requestPath = null)
    {
        return new SysLogWarning
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
