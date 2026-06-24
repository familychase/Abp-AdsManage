namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 批量删除广告系列单项结果
/// </summary>
public class BatchDeleteCampaignResultDto
{
    /// <summary>
    /// 广告系列编号
    /// </summary>
    public string CampaignNo { get; set; } = string.Empty;

    /// <summary>
    /// 是否删除成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误日志（仅失败时有值）
    /// </summary>
    public string? ErrorMessage { get; set; }
}
