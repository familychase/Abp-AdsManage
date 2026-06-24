namespace Ads.Automation.Application.Contracts.Entity.Menus;

/// <summary>
/// 获取菜单列表Input
/// </summary>
public class GetSysMenusListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 菜单名称过滤
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// 菜单类型过滤
    /// </summary>
    public SysMenuType? MenuType { get; set; }

    /// <summary>
    /// 父菜单ID过滤
    /// </summary>
    public long? ParentId { get; set; }
}
