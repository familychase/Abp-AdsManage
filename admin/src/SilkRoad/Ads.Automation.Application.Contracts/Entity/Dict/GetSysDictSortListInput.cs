namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典类 列表查询 Input
/// </summary>
public class GetSysDictSortListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 搜索关键字（模糊匹配编码/名称）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 字典类型筛选
    /// </summary>
    public SysDictSortType? DictSortType { get; set; }
}
