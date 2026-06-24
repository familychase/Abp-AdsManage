namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 区域/城市出参（Meta targeting search 结果）
/// </summary>
public class MediaRegionDto
{
    /// <summary>
    /// 区域/城市 Key（投放时使用）
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 区域/城市名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 地域类型（region / city / country / country_group）
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// 国家代码（如 CN、US）
    /// </summary>
    public string? CountryCode { get; set; }

    /// <summary>
    /// 国家名称
    /// </summary>
    public string? CountryName { get; set; }

    /// <summary>
    /// 所属区域名称（城市时有值）
    /// </summary>
    public string? Region { get; set; }

    /// <summary>
    /// 所属区域 ID（城市时有值）
    /// </summary>
    public long? RegionId { get; set; }

    /// <summary>
    /// 是否支持下钻到区域
    /// </summary>
    public bool SupportsRegion { get; set; }

    /// <summary>
    /// 是否支持下钻到城市
    /// </summary>
    public bool SupportsCity { get; set; }

    /// <summary>
    /// 国家组包含的国家代码列表（type=country_group 时有值）
    /// </summary>
    public List<string>? CountryCodes { get; set; }
}
