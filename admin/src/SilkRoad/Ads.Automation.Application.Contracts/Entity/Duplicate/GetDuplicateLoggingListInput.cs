using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Duplicate;

/// <summary>
/// 复制日志列表查询入参
/// </summary>
public class GetDuplicateLoggingListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 复制来源（精确匹配）
    /// </summary>
    public DuplicateSource? DuplicateSource { get; set; }

    /// <summary>
    /// 复制状态（精确匹配）
    /// </summary>
    public DuplicateState? State { get; set; }

    /// <summary>
    /// 来源广告对象编号（模糊搜索）
    /// </summary>
    public string? AdObjectNo { get; set; }

    /// <summary>
    /// 来源账户编号（精确匹配）
    /// </summary>
    public string? AccountNo { get; set; }

    /// <summary>
    /// 目标账户号（精确匹配）
    /// </summary>
    [JsonIgnore]
    public string? DuplicateAccountNo { get; set; }
}
