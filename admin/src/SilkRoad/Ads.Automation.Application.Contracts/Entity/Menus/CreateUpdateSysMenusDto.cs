namespace Ads.Automation.Application.Contracts.Entity.Menus;

/// <summary>
/// 创建/修改菜单信息Dto
/// </summary>
public class CreateUpdateSysMenusDto
{
    /// <summary>
    /// 父菜单ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [StringLength(128)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 菜单名称-英文
    /// </summary>
    [Required]
    public string NameEn { get; set; } = string.Empty;

    /// <summary>
    /// 前端路由
    /// </summary>
    [StringLength(256)]
    public string Route { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 类型（目录、菜单、按钮）
    /// </summary>
    [Required]
    public SysMenuType MenuType { get; set; }

    /// <summary>
    /// 权限标识
    /// </summary>
    public string? PermissionCode { get; set; }

    /// <summary>
    /// 菜单路由路径
    /// </summary>
    public string? MenuPath { get; set; }

    /// <summary>
    /// 前端组件名称
    /// </summary>
    [StringLength(128)]
    public string ComponentName { get; set; } = string.Empty;

    /// <summary>
    /// 前端组件路径
    /// </summary>
    [StringLength(256)]
    public string ComponentPath { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否显示
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Remark { get; set; }
}
