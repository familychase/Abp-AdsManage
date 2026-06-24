namespace Ads.Automation.Application.Contracts.Entity.SysRoles;

/// <summary>
/// 创建/修改角色信息Dto
/// </summary>
public class CreateUpdateSysRolesDto
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 关联的菜单Id列表
    /// </summary>
    public List<long> MenuIds { get; set; } = new List<long>();
}
