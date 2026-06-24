
using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Departments;

/// <summary>
/// 部门信息 AppService 实现
/// </summary>
public class SysDepartmentAppService : ApplicationService, ISysDepartmentAppService
{
    private readonly IBaseRepository<SysDepartment> _departmentRepository;
    private readonly IBaseRepository<SysUser> _userRepository;

    public SysDepartmentAppService(
        IBaseRepository<SysDepartment> departmentRepository,
        IBaseRepository<SysUser> userRepository)
    {
        _departmentRepository = departmentRepository;
        _userRepository = userRepository;
    }

    public async Task<SysDepartmentDto> CreateAsync(CreateUpdateSysDepartmentDto input)
    {
        var model = SysDepartment.Create(
            input.ParentId,
            input.DeptName,
            input.Sort,
            input.AliasName,
            input.Path,
            input.Remark
        );

        await _departmentRepository.InsertAsync(model);
        return ObjectMapper.Map<SysDepartment, SysDepartmentDto>(model);
    }

    public async Task DeleteAsync(long id)
    {
        // 1. 存在未被删除的下级部门时，不允许删除
        var childQuery = await _departmentRepository.GetQueryableAsync();
        var hasChild = await childQuery.AnyAsync(d => d.ParentId == id && !d.IsDeleted);
        if (hasChild)
            throw new BusinessException("Department:HasChildDept");

        // 2. 存在未被删除的用户成员时，不允许删除
        var userQuery = await _userRepository.GetQueryableAsync();
        var hasUser = await userQuery.AnyAsync(u => u.DepartmentId == id && !u.IsDeleted);
        if (hasUser)
            throw new BusinessException("Department:HasUser");

        await _departmentRepository.DeleteAsync(d => d.Id == id);
    }

    public async Task<SysDepartmentDto> GetAsync(long id)
    {
        var department = await _departmentRepository.GetAsync(d => d.Id == id);
        return ObjectMapper.Map<SysDepartment, SysDepartmentDto>(department);
    }

    public async Task<PagedResultDto<SysDepartmentDto>> GetListAsync(GetSysDepartmentListInput input)
    {
        var query = await _departmentRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(d => d.DeptName.Contains(input.FilterText));
        }

        if (input.ParentId.HasValue)
        {
            query = query.Where(d => d.ParentId == input.ParentId.Value);
        }

        var totalCount = query.Count();

        var list = await query
            .OrderBy(d => d.Sort)
            .ThenBy(d => d.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount).ToListAsync();

        var items = ObjectMapper.Map<List<SysDepartment>, List<SysDepartmentDto>>(list);

        var parentIds = list.Select(s => s.ParentId).Distinct().ToList();
        var parentDept = (await _departmentRepository.GetListAsync(e => parentIds.Contains(e.Id)))
            .ToDictionary(k => k.Id, v => v.DeptName);

        foreach (var item in items)
        {
            item.ParentName = item.ParentId > 0 ? parentDept.FirstOrDefault(e => e.Key == item.ParentId).Value : "";
        }

        return new PagedResultDto<SysDepartmentDto>(totalCount, items);
    }

    public async Task<List<SysDepartmentDto>> GetTreeAsync(GetSysDepartmentListInput input)
    {
        var query = await _departmentRepository.GetQueryableAsync();
        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(d => d.DeptName.Contains(input.FilterText));
        }

        var allDepartments = await query
            .OrderBy(d => d.Sort)
            .ThenBy(d => d.Id)
            .ToListAsync();

        var dtos = ObjectMapper.Map<List<SysDepartment>, List<SysDepartmentDto>>(allDepartments);

        // 按 ParentId 分组，递归不限层级深度
        var childrenMap = dtos.GroupBy(d => d.ParentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var tree = BuildTree(0, childrenMap);
        return new List<SysDepartmentDto>(){
            new SysDepartmentDto
            {
                Id = 0,
                ParentId = -1,
                DeptName = "总部门",
                Children = tree
            }
        };

        // 递归构建子树
        List<SysDepartmentDto> BuildTree(long parentId, Dictionary<long, List<SysDepartmentDto>> childrenMap)
        {
            if (!childrenMap.TryGetValue(parentId, out var children))
                return new List<SysDepartmentDto>();

            foreach (var child in children)
            {
                child.Children = BuildTree(child.Id, childrenMap);
            }

            return children;
        }
    }

    public async Task<SysDepartmentDto> UpdateAsync(long id, CreateUpdateSysDepartmentDto input)
    {
        var department = await _departmentRepository.GetAsync(d => d.Id == id);

        department.SetName(input.DeptName);
        department.SetParent(input.ParentId);
        department.SetAlias(input.AliasName);
        department.SetPath(input.Path);
        department.SetRemark(input.Remark);

        await _departmentRepository.UpdateAsync(department);
        return ObjectMapper.Map<SysDepartment, SysDepartmentDto>(department);
    }
}
