using Ads.Automation.Domain.Menu;

namespace Ads.Automation.Application.Contracts.Entity.Menus;

/// <summary>
/// 菜单树节点Dto
/// </summary>
public class MenuTreeNodeDto : SysMenusMapDto
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public List<string> PermissionCode { get; set; } = default!;

    /// <summary>
    /// 类型（目录、菜单、按钮）
    /// </summary>
    public SysMenuType MenuType { get; set; }

    /// <summary>
    /// 子菜单列表
    /// </summary>
    public new List<MenuTreeNodeDto> Children { get; set; } = new();
}
