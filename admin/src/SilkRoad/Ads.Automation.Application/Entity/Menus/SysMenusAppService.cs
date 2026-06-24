

using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Menus;

/// <summary>
/// 菜单信息AppService实现
/// </summary>
public class SysMenusAppService : ApplicationService, ISysMenusAppService
{
    private readonly IBaseRepository<SysMenus> _menusRepository;

    public SysMenusAppService(IBaseRepository<SysMenus> menusRepository)
    {
        _menusRepository = menusRepository;
    }

    public async Task<SysMenusDto> CreateAsync(CreateUpdateSysMenusDto input)
    {
        var model = SysMenus.Create(
            input.Name,
            input.NameEn,
            input.ParentId ?? 0,
            input.Route,
            input.Icon ?? string.Empty,
            input.MenuType,
            input.PermissionCode ?? string.Empty,
            input.ComponentName,
            input.ComponentPath,
            input.Sort,
            input.Visible,
            input.Remark ?? string.Empty,
            DateTime.Now,
            0,           // creatorId, 后续从当前用户获取
            null,         // lastModificationTime
            null          // lastModifierId
        );

        await _menusRepository.InsertAsync(model);
        return ObjectMapper.Map<SysMenus, SysMenusDto>(model);
    }

    public async Task DeleteAsync(long id)
    {
        var menu = await _menusRepository.GetAsync(m => m.Id == id);

        // 目录类型菜单：存在下级未删除菜单时，不允许删除
        if (menu.MenuType == SysMenuType.DIRECTORY)
        {
            var childQuery = await _menusRepository.GetQueryableAsync();
            var hasChild = await childQuery.AnyAsync(m => m.ParentId == id && !m.IsDeleted);
            if (hasChild)
                throw new BusinessException("Menu:HasChildMenu");
        }

        await _menusRepository.DeleteAsync(m => m.Id == id);
    }

    public async Task<SysMenusDto> GetAsync(long id)
    {
        var menu = await _menusRepository.GetAsync(m => m.Id == id);
        return ObjectMapper.Map<SysMenus, SysMenusDto>(menu);
    }

    public async Task<PagedResultDto<SysMenusDto>> GetListAsync(GetSysMenusListInput input)
    {
        var query = await _menusRepository.GetQueryableAsync();
        query = query.Where(e => e.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(m => m.Name.Contains(input.FilterText));
        }

        if (input.MenuType.HasValue)
        {
            query = query.Where(m => m.MenuType == input.MenuType.Value);
        }

        if (input.ParentId.HasValue)
        {
            query = query.Where(m => m.ParentId == input.ParentId.Value);
        }

        var totalCount = query.Count();

        query = query
            .OrderBy(m => m.Sort)
            .ThenBy(m => m.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = ObjectMapper.Map<List<SysMenus>, List<SysMenusDto>>(query.ToList());

        return new PagedResultDto<SysMenusDto>(totalCount, items);
    }

    public async Task<List<SysMenusDto>> GetTreeAsync(GetSysMenusListInput input)
    {
        var query = await _menusRepository.GetQueryableAsync();
        query = query.Where(e => e.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(m => m.Name.Contains(input.FilterText));
        }

        if (input.MenuType.HasValue)
        {
            query = query.Where(m => m.MenuType == input.MenuType.Value);
        }

        if (input.ParentId.HasValue)
        {
            query = query.Where(m => m.ParentId == input.ParentId.Value);
        }

        var allMenus = await query
            .OrderBy(m => m.Sort)
            .ThenBy(m => m.Id)
            .ToListAsync();

        var dtos = ObjectMapper.Map<List<SysMenus>, List<SysMenusDto>>(allMenus);

        // 按 ParentId 分组，递归不限层级深度
        var childrenMap = dtos.GroupBy(d => d.ParentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var tree = BuildTree(0, childrenMap);
        return new List<SysMenusDto>()
        {
            new SysMenusDto
            {
                Id = 0,
                ParentId = -1,
                Name = "总菜单",
                Children = tree
            }
        };

        // 递归构建子树
        List<SysMenusDto> BuildTree(long parentId, Dictionary<long, List<SysMenusDto>> childrenMap)
        {
            if (!childrenMap.TryGetValue(parentId, out var children))
                return new List<SysMenusDto>();

            foreach (var child in children)
            {
                child.Children = BuildTree(child.Id, childrenMap);
            }

            return children;
        }
    }

    public async Task<SysMenusDto> UpdateAsync(long id, CreateUpdateSysMenusDto input)
    {
        var menu = await _menusRepository.GetAsync(m => m.Id == id);

        menu.ParentId = input.ParentId ?? 0;
        menu.Name = input.Name;
        menu.NameEn = input.NameEn;
        menu.Route = input.Route;
        menu.Icon = input.Icon ?? string.Empty;
        menu.MenuType = input.MenuType;
        menu.PermissionCode = input.PermissionCode ?? string.Empty;
        menu.ComponentName = input.ComponentName;
        menu.ComponentPath = input.ComponentPath;
        menu.Sort = input.Sort;
        menu.Visible = input.Visible;
        menu.Remark = input.Remark ?? string.Empty;
        menu.LastModificationTime = DateTime.Now;

        await _menusRepository.UpdateAsync(menu);
        return ObjectMapper.Map<SysMenus, SysMenusDto>(menu);
    }
}
