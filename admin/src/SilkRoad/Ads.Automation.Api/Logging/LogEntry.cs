namespace Ads.Automation.Api.Logging;

/// <summary>
/// 日志条目 DTO
/// </summary>
public class LogEntry
{
    public string Level { get; set; } = string.Empty;
    public string Logger { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? RequestPath { get; set; }
}
