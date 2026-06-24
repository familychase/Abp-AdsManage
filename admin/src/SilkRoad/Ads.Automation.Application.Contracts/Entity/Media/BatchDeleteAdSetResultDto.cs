namespace Ads.Automation.Application.Contracts.Entity.Media;

/// <summary>
/// 批量删除广告组单项结果
/// </summary>
public class BatchDeleteAdSetResultDto
{
    /// <summary>
    /// 广告组编号
    /// </summary>
    public string AdSetNo { get; set; } = string.Empty;

    /// <summary>
    /// 是否删除成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息（仅失败时有值）
    /// </summary>
    public string? ErrorMessage { get; set; }
}
