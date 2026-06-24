namespace Ads.Automation.Domain.Log;

/// <summary>
/// 常规日志实体（Information / Debug / Trace）
/// </summary>
public class SysLogInfo : AggregateRootEntity, IHasCreationTimeEntity
{
    public string Level { get; private set; } = string.Empty;
    public string Logger { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? RequestPath { get; private set; }

    public long CreatorId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;

    private SysLogInfo() { }

    public static SysLogInfo Create(long id, string level, string logger, string message, string? requestPath = null)
    {
        return new SysLogInfo
        {
            Id = id,
            Level = level,
            Logger = logger,
            Message = message,
            RequestPath = requestPath,
            CreationTime = DateTime.Now
        };
    }
}
