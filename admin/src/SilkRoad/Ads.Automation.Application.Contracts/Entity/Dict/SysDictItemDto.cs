namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典项 DTO（基类），用于 web_list / 下拉框等简单场景
/// </summary>
public class SysDictItemDto
{
    public long Id { get; set; }
    public long DictSortId { get; set; }
    public string DictItemCode { get; set; } = string.Empty;
    public string DictItemName { get; set; } = string.Empty;
    public string DictItemValue { get; set; } = string.Empty;
}
