using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Log;

/// <summary>
/// 错误日志查询入参
/// </summary>
public class GetSysLogErrorListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 日志级别（精确匹配，如 Error / Critical）
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// 日志来源（Logger，模糊匹配）
    /// </summary>
    public string? Logger { get; set; }

    /// <summary>
    /// 请求路径（精确匹配）
    /// </summary>
    public string? RequestPath { get; set; }

    /// <summary>
    /// 关键字：同时模糊匹配 Message 和 Exception
    /// </summary>
    public string? Keyword { get; set; }
}
