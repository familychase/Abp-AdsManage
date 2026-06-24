using System.ComponentModel.DataAnnotations;

namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 批量删除广告系列入参
/// </summary>
public class BatchDeleteCampaignsInput
{
    /// <summary>
    /// 广告账户编号
    /// </summary>
    [Required]
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告系列编号列表（Meta Campaign ID）
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<string> CampaignNos { get; set; } = new();
}
