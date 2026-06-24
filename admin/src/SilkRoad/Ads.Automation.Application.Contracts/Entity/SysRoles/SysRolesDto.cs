namespace Ads.Automation.Application.Contracts.Entity.SysRoles;

/// <summary>
/// 角色信息Dto
/// </summary>
public class SysRolesDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public int Sort { get; set; }
    public string? Remark { get; set; }
    public string CreationTime { get; set; } = default!;
    /// <summary>
    /// 关联的菜单Id列表
    /// </summary>
    public List<long> MenuIds { get; set; } = new List<long>();
}
