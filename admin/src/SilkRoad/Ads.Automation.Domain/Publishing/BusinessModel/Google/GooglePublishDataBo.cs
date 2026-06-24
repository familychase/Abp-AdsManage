namespace Ads.Automation.Domain.Publishing.BusinessModel.Google;

/// <summary>
/// Google 平台的发布对象视图模型
/// </summary>
public sealed class GooglePublishDataBo
{
    /// <summary>广告系列数据</summary>
    public GoogleCampaignBo CampaignData { get; set; } = null!;

    /// <summary>受众数据</summary>
    public GoogleAudienceBo AudienceData { get; set; } = null!;
}

/// <summary>
/// Google 广告系列 BO
/// </summary>
public class GoogleCampaignBo
{
    /// <summary>优化目标（如 MAXIMIZE_CONVERSIONS）</summary>
    public string OptimizationGoal { get; set; } = null!;

    /// <summary>预算值</summary>
    public long BudgetValue { get; set; }
}

/// <summary>
/// Google 受众 BO
/// </summary>
public class GoogleAudienceBo
{
    /// <summary>包含的国家/地区列表</summary>
    public List<GoogleAreaBo>? PositiveCountryCodes { get; set; }

    /// <summary>语言列表</summary>
    public List<string>? Languages { get; set; }
}

/// <summary>
/// Google 区域模型
/// </summary>
public class GoogleAreaBo
{
    /// <summary>国家代码</summary>
    public string CountryCode { get; set; } = null!;
}
