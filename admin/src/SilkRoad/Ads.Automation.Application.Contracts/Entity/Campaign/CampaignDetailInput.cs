using System.ComponentModel.DataAnnotations;
using Ads.Automation.Domain.Shared.Localization;

namespace Ads.Automation.Application.Contracts.Entity.Campaign;

/// <summary>
/// Campaign detail query input
/// </summary>
public class CampaignDetailInput
{
    /// <summary>
    /// Campaign number (Meta Campaign ID)
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(V), ErrorMessageResourceName = nameof(V.CampaignNoRequired))]
    public string CampaignNo { get; set; } = string.Empty;

    /// <summary>
    /// Account number (used to verify account ownership)
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(V), ErrorMessageResourceName = nameof(V.AccountNoRequired))]
    public string AccountNo { get; set; } = string.Empty;
}
