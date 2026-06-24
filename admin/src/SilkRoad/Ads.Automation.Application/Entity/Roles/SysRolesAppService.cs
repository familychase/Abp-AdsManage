

using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Roles;

/// <summary>
/// 角色信息AppService实现
/// </summary>
public class SysRolesAppService : ApplicationService, ISysRolesAppService
{
    private readonly IBaseRepository<SysRoles> _rolesRepository;
    private readonly IBaseRepository<SysRolePower> _rolePowerRepository;
    private readonly IBaseRepository<SysUser> _userRepository;

    public SysRolesAppService(
        IBaseRepository<SysRoles> rolesRepository,
        IBaseRepository<SysRolePower> rolePowerRepository,
        IBaseRepository<SysUser> userRepository)
    {
        _rolesRepository = rolesRepository;
        _rolePowerRepository = rolePowerRepository;
        _userRepository = userRepository;
    }

    public async Task<SysRolesDto> CreateAsync(CreateUpdateSysRolesDto input)
    {
        var model = SysRoles.Create(
            input.Name,
            input.Sort,
            input.Remark
        );

        // 通过导航属性保存角色关联的菜单权限
        foreach (var menuId in input.MenuIds)
        {
            model.RolePowers.Add(SysRolePower.Create(model.Id, menuId));
        }

        await _rolesRepository.InsertAsync(model);

        return ObjectMapper.Map<SysRoles, SysRolesDto>(model);
    }

    public async Task DeleteAsync(long id)
    {
        // 存在未被删除的用户绑定了该角色时，不允许删除
        var userQuery = await _userRepository.GetQueryableAsync();
        var hasUser = await userQuery.AnyAsync(u => u.RoleId == id && !u.IsDeleted);
        if (hasUser)
            throw new BusinessException("Role:HasUser");

        await _rolesRepository.DeleteAsync(r => r.Id == id);
    }

    public async Task<SysRolesDto> GetAsync(long id)
    {
        var roleQuery = await _rolesRepository.GetQueryableAsync();
        var role = roleQuery.Include(r => r.RolePowers).First(r => r.Id == id && r.IsDeleted == false);

        var dto = ObjectMapper.Map<SysRoles, SysRolesDto>(role);
        dto.MenuIds = role.RolePowers.Select(rp => rp.MenuId).ToList();

        return dto;
    }

    public async Task<PagedResultDto<SysRolesDto>> GetListAsync(GetSysRolesListInput input)
    {
        IQueryable<SysRoles> query = await _rolesRepository.GetQueryableAsync();
        query = query.Where(e => e.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(r => r.Name.Contains(input.FilterText));
        }

        var totalCount = await query.CountAsync();

        var roles = await query
            .OrderBy(r => r.Sort)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        var items = ObjectMapper.Map<List<SysRoles>, List<SysRolesDto>>(roles);

        return new PagedResultDto<SysRolesDto>(totalCount, items);
    }

    public async Task<SysRolesDto> UpdateAsync(long id, CreateUpdateSysRolesDto input)
    {
        var role = await _rolesRepository.GetAsync(r => r.Id == id);

        // 更新角色基本信息
        role.SetName(input.Name);
        role.SetSort(input.Sort);
        role.SetRemark(input.Remark);
        await _rolesRepository.UpdateAsync(role);

        // 中间表：比对后按需增删，避免全量删插
        await SyncRolePowersAsync(id, input.MenuIds.Where(m => m > 0).ToList());

        return ObjectMapper.Map<SysRoles, SysRolesDto>(role);
    }

    /// <summary>
    /// 同步角色菜单权限：比对已有和新传入的 MenuId，保留交集、新增差集、删除差集
    /// </summary>
    private async Task SyncRolePowersAsync(long roleId, List<long> newMenuIds)
    {
        var existingPowers = await _rolePowerRepository.GetListAsync(rp => rp.RoleId == roleId);
        var existingMenuIds = existingPowers.Select(rp => rp.MenuId).ToHashSet();
        var newSet = newMenuIds.ToHashSet();

        // 删除：在旧集合但不在新集合
        var toDelete = existingPowers.Where(rp => !newSet.Contains(rp.MenuId)).ToList();
        foreach (var item in toDelete)
        {
            await _rolePowerRepository.DeleteAsync(item);
        }

        // 添加：在新集合但不在旧集合
        var toAdd = newMenuIds.Where(m => !existingMenuIds.Contains(m)).ToList();
        foreach (var menuId in toAdd)
        {
            await _rolePowerRepository.InsertAsync(SysRolePower.Create(roleId, menuId));
        }
    }
}
