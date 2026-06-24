namespace Ads.Automation.Application.Contracts.Entity.Departments;

/// <summary>
/// 部门信息 Interface
/// </summary>
public interface ISysDepartmentAppService : IApplicationService
{
    /// <summary>
    /// 获取部门信息
    /// </summary>
    Task<SysDepartmentDto> GetAsync(long id);

    /// <summary>
    /// 获取部门列表
    /// </summary>
    Task<PagedResultDto<SysDepartmentDto>> GetListAsync(GetSysDepartmentListInput input);

    /// <summary>
    /// 创建部门
    /// </summary>
    Task<SysDepartmentDto> CreateAsync(CreateUpdateSysDepartmentDto input);

    /// <summary>
    /// 修改部门
    /// </summary>
    Task<SysDepartmentDto> UpdateAsync(long id, CreateUpdateSysDepartmentDto input);

    /// <summary>
    /// 删除部门
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 获取部门树形结构列表
    /// </summary>
    Task<List<SysDepartmentDto>> GetTreeAsync(GetSysDepartmentListInput input);
}
