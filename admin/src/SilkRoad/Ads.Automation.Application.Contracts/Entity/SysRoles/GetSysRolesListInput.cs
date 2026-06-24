namespace Ads.Automation.Application.Contracts.Entity.SysRoles;

/// <summary>
/// 获取角色列表Input
/// </summary>
public class GetSysRolesListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 角色名称过滤
    /// </summary>
    public string? FilterText { get; set; }
}
