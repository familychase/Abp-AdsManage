namespace Ads.Automation.Application.Contracts.Entity.Menus;

/// <summary>
/// 菜单信息Dto
/// </summary>
public class SysMenusDto
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
    /// 菜单名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 菜单名称-英文
    /// </summary>
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// 前端路由
    /// </summary>
    public string Route { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; } = string.Empty;

    /// <summary>
    /// 类型（目录=1, 菜单=2, 按钮=3）
    /// </summary>
    public SysMenuType MenuType { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string PermissionCode { get; set; } = default!;

    /// <summary>
    /// 前端组件名称
    /// </summary>
    public string ComponentName { get; set; } = default!;

    /// <summary>
    /// 前端组件路径
    /// </summary>
    public string ComponentPath { get; set; } = default!;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否显示
    /// </summary>
    public bool Visible { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 子菜单列表（树形结构）
    /// </summary>
    public List<SysMenusDto> Children { get; set; } = new();
}
