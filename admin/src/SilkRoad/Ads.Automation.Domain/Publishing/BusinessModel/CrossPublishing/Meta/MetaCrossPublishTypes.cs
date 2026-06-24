namespace Ads.Automation.Domain.Publishing.BusinessModel.CrossPublishing.Meta;

/// <summary>
/// 跨平台发布区域/城市模型（包含Key、国家代码、名称等）
/// </summary>
public class MetaCrossPublishDataAreaGroupAreaBo
{
    /// <summary>区域/城市 Key（投放定位使用）</summary>
    public string? Key { get; set; }

    /// <summary>地域类型（country / region / city / country_group）</summary>
    public string? Type { get; set; }

    /// <summary>区域/城市显示名称</summary>
    public string? Name { get; set; }

    /// <summary>国家代码（如 CN、US）</summary>
    public string? CountryCode { get; set; }
}

/// <summary>
/// 跨平台发布区域包含/排除视图模型
/// </summary>
public class MetaCrossPublishDataAreaGroupAreaViewBo
{
    /// <summary>包含的区域/城市列表</summary>
    public List<MetaCrossPublishDataAreaGroupAreaBo> Includes { get; set; } = new();

    /// <summary>排除的区域/城市列表</summary>
    public List<MetaCrossPublishDataAreaGroupAreaBo> Excludes { get; set; } = new();
}

/// <summary>
/// 跨平台发布子版位模型（Facebook / Instagram / AudienceNetwork / Messenger）
/// </summary>
public class MetaCrossPublishDataPositionChildrenBo
{
    /// <summary>Facebook 子版位列表</summary>
    public List<string>? Facebook { get; set; }

    /// <summary>Instagram 子版位列表</summary>
    public List<string>? Instagram { get; set; }

    /// <summary>Audience Network 子版位列表</summary>
    public List<string>? AudienceNetwork { get; set; }

    /// <summary>Messenger 子版位列表</summary>
    public List<string>? Messenger { get; set; }
}
