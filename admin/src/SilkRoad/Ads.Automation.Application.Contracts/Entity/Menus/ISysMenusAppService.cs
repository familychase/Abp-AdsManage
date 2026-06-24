namespace Ads.Automation.Application.Contracts.Entity.Menus;

/// <summary>
/// 菜单信息Interface
/// </summary>
public interface ISysMenusAppService : IApplicationService
{
    /// <summary>
    /// 获取菜单信息
    /// </summary>
    Task<SysMenusDto> GetAsync(long id);

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    Task<PagedResultDto<SysMenusDto>> GetListAsync(GetSysMenusListInput input);

    /// <summary>
    /// 创建菜单
    /// </summary>
    Task<SysMenusDto> CreateAsync(CreateUpdateSysMenusDto input);

    /// <summary>
    /// 修改菜单
    /// </summary>
    Task<SysMenusDto> UpdateAsync(long id, CreateUpdateSysMenusDto input);

    /// <summary>
    /// 删除菜单
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 获取菜单树形结构列表
    /// </summary>
    Task<List<SysMenusDto>> GetTreeAsync(GetSysMenusListInput input);
}
