namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 字典项树节点 DTO，用于 GetTreeAsync
/// </summary>
public class SysDictItemTreeNodeDto : SysDictItemDto
{
    public long ParentId { get; set; }

    /// <summary>
    /// 子级
    /// </summary>
    public List<SysDictItemTreeNodeDto> Children { get; set; } = new();
}
