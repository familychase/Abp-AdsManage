namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 公共主页出参（实时 Meta API 查询结果）
/// </summary>
public class MediaPageDto
{
    /// <summary>
    /// 主页编号（Meta Page ID）
    /// </summary>
    public string PageNo { get; set; } = string.Empty;

    /// <summary>
    /// 主页名称
    /// </summary>
    public string PageName { get; set; } = string.Empty;

    /// <summary>
    /// 主页分类
    /// </summary>
    public string? Category { get; set; }
}
