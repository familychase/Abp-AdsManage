namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 复制明细 DTO
/// </summary>
public class AdsDuplicateDetailDto
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 关联的复制日志 ID
    /// </summary>
    public long LogId { get; set; }

    /// <summary>
    /// 迭代序号（第几次复制，1..CopyNumber）
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 广告层级：广告系列(CAMPAIGN) 或 广告组(AD_SET)
    /// </summary>
    public AdObjectLevel AdObjectLevel { get; set; }

    /// <summary>
    /// 新创建的广告对象编号
    /// </summary>
    public string AdObjectNo { get; set; } = string.Empty;

    /// <summary>
    /// 本轮状态
    /// </summary>
    public DuplicateState State { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 创建内容详情
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
