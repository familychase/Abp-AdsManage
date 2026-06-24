using Ads.Automation.Domain.Shared.Enums.Publishing;

namespace Ads.Automation.Domain.Publishing.BusinessModel.Meta;

/// <summary>
/// Meta 归因窗口模型（点击归因、浏览归因触点天数）
/// </summary>
public class MetaAdsPublishAttributionSpecBo
{
    /// <summary>点击归因窗口（天）</summary>
    public string? ClickThrough { get; set; }

    /// <summary>浏览归因窗口（天）</summary>
    public string? ViewThrough { get; set; }
}
