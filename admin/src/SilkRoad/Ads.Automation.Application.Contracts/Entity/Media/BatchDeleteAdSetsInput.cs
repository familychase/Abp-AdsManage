using System.ComponentModel.DataAnnotations;

namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 批量删除广告组入参
/// </summary>
public class BatchDeleteAdSetsInput
{
    /// <summary>
    /// 广告账户编号
    /// </summary>
    [Required]
    public string AccountNo { get; set; } = string.Empty;

    /// <summary>
    /// 广告组编号列表（Meta AdSet ID）
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<string> AdSetNos { get; set; } = new();
}
