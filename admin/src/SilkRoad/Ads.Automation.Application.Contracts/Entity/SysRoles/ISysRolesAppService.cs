namespace Ads.Automation.Application.Contracts.Entity.SysRoles;

/// <summary>
/// 角色信息Interface
/// </summary>
public interface ISysRolesAppService : IApplicationService
{
    /// <summary>
    /// 获取角色信息
    /// </summary>
    Task<SysRolesDto> GetAsync(long id);

    /// <summary>
    /// 获取角色列表
    /// </summary>
    Task<PagedResultDto<SysRolesDto>> GetListAsync(GetSysRolesListInput input);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<SysRolesDto> CreateAsync(CreateUpdateSysRolesDto input);

    /// <summary>
    /// 修改角色
    /// </summary>
    Task<SysRolesDto> UpdateAsync(long id, CreateUpdateSysRolesDto input);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task DeleteAsync(long id);
}
