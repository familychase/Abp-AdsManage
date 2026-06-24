namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 获取区域/城市列表入参
/// </summary>
public class GetRegionListInput
{
    /// <summary>
    /// 搜索关键字
    /// </summary>
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// 地域类型（支持：region、city、country、country_group）
    /// 多个类型用英文逗号分隔，默认返回 region 和 city
    /// </summary>
    public List<string> LocationTypes { get; set; } = new() { "region", "city" };

    /// <summary>
    /// 返回条数限制，默认 20
    /// </summary>
    public int Limit { get; set; } = 20;

    /// <summary>
    /// 语言区域（如 zh_CN、en_US），不传则使用平台默认语言
    /// </summary>
    public string? Locale { get; set; }
}
