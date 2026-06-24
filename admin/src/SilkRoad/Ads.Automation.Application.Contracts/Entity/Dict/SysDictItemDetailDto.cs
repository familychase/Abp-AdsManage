namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 字典项详情 DTO，用于 GetAsync / GetListAsync / CreateAsync / UpdateAsync 返回
/// </summary>
public class SysDictItemDetailDto : SysDictItemDto
{
    public long ParentId { get; set; }
    public string DictSortName { get; set; } = string.Empty;
    public string DictItemNameEN { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int Ordinal { get; set; }
    public DictItemValueType ItemType { get; set; }
    public bool IsProduction { get; set; }
    public string CreationTime { get; set; } = string.Empty;
}
