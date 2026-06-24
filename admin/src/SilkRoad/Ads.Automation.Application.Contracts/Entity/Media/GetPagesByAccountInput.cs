using System.ComponentModel.DataAnnotations;
using Ads.Automation.Domain.Shared.Localization;

namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 根据账户获取公共主页入参
/// </summary>
public class GetPagesByAccountInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 广告账户编号（如 act_2819618115098509）
    /// </summary>
    [Required(ErrorMessageResourceType = typeof(V), ErrorMessageResourceName = nameof(V.AccountNoRequired))]
    public string AccountNo { get; set; } = string.Empty;
}
