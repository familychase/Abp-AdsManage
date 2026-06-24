namespace Ads.Automation.Application.Contracts.Log;

/// <summary>
/// 错误日志 DTO
/// </summary>
public class SysLogErrorDto
{
    /// <summary>
    /// 主键（雪花 ID）
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 日志级别（Error / Critical）
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// 日志来源（Logger 名称）
    /// </summary>
    public string Logger { get; set; } = string.Empty;

    /// <summary>
    /// 日志消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 异常详情
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// 创建时间（北京时间，格式 yyyy-MM-dd HH:mm:ss）
    /// </summary>
    public string CreationTime { get; set; } = string.Empty;
}
