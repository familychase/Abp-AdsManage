namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典项 列表查询 Input
/// </summary>
public class GetSysDictItemListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 字典类Id（必填）
    /// </summary>
    public long? DictSortId { get; set; }

    /// <summary>
    /// 搜索关键字（模糊匹配编码/名称）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 父级Id（查询子级用）
    /// </summary>
    public long? ParentId { get; set; }
}
