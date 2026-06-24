using System.Linq.Expressions;
using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Users
{
    /// <summary>
    /// 用户信息AppService实现
    /// </summary>
    public class SysUserAppService : ApplicationService, ISysUserAppService
    {
        private readonly IBaseRepository<SysUser> _userManager;
        private readonly IBaseRepository<SysMenus> _menuRepository;
        private readonly IBaseRepository<SysDepartment> _deptRepository;
        private readonly IBaseRepository<SysRolePower> _rolePowerRepository;
        private readonly LocalizationContext _localizationContext;

        public SysUserAppService(
            IBaseRepository<SysUser> userManager,
            IBaseRepository<SysMenus> menuRepository,
            IBaseRepository<SysDepartment> deptRepository,
            IBaseRepository<SysRolePower> rolePowerRepository,
            LocalizationContext localizationContext)
        {
            _userManager = userManager;
            _menuRepository = menuRepository;
            _deptRepository = deptRepository;
            _rolePowerRepository = rolePowerRepository;
            _localizationContext = localizationContext;
        }

        public async Task<SysUserDto> CreateAsync(CreateUpdateSysUserDto input)
        {
            if (await _userManager.AnyAsync(u => u.UserCode == input.UserCode))
            {
                throw new BusinessException("User:UserCodeExists").WithData("0", input.UserCode);
            }

            var model = SysUser.Create(
                IdGenerator.GetNextId(),
                input.UserCode,
                input.UserCode,
                input.DepartmentId,
                input.RoleId,
                input.Status,
                input.IsAdmin,
                input.IsTeamAdmin,
                input.AliasName,
                input.PhoneNumber,
                input.Email
            );

            await _userManager.InsertAsync(model);
            return ObjectMapper.Map<SysUser, SysUserDto>(model);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var model = await _userManager.GetAsync(id);
            if (model == null)
            {
                throw new BusinessException("User:NotFound");
            }

            await _userManager.DeleteAsync(model);
            return true;
        }

        public async Task<SysUserDto> GetAsync(long id)
        {
            var customer = await _userManager.GetAsync(c => c.Id == id && c.IsDeleted == false);
            return ObjectMapper.Map<SysUser, SysUserDto>(customer);
        }

        public async Task<PagedResultDto<SysUserDto>> GetListAsync(GetSysUserListInput input)
        {
            var queryable = await _userManager.GetQueryableAsync();
            Expression<Func<SysUser, bool>> filter = u => u.IsDeleted == false;

            if (!string.IsNullOrWhiteSpace(input.FilterText))
            {
                filter = filter.And(u =>
                    u.UserCode.Contains(input.FilterText) ||
                    u.UserCode.Contains(input.FilterText) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(input.FilterText)));
            }

            if (input.DepartmentId.HasValue && input.DepartmentId > 0)
            {
                filter = filter.And(u => u.DepartmentId == input.DepartmentId.Value);
            }

            if (input.IsAdmin.HasValue)
            {
                filter = filter.And(u => u.IsAdmin == input.IsAdmin.Value);
            }

            if (input.Status.HasValue)
            {
                filter = filter.And(u => u.Status == input.Status.Value);
            }

            var totalCount = await _userManager.CountAsync(filter);
            if (totalCount == 0)
                return new PagedResultDto<SysUserDto>(0, new List<SysUserDto>());

            var list = queryable.Where(filter)
                .OrderBy(u => u.Id)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            var items = ObjectMapper.Map<List<SysUser>, List<SysUserDto>>(list);
            var deptId = items.Select(s => s.DepartmentId).Distinct().ToList();
            var deptDics = (await _deptRepository.GetListAsync(d => deptId.Contains(d.Id)))
                .ToDictionary(d => d.Id);

            foreach (var item in items)
            {
                item.DepartmentName = deptDics.TryGetValue(item.DepartmentId, out var dept)
                    ? dept.DeptName ?? string.Empty
                    : string.Empty;
            }

            return new PagedResultDto<SysUserDto>(totalCount, items);
        }

        public async Task<bool> ResetPasswordAsync(long id)
        {
            var user = await _userManager.GetAsync(id);
            if (user == null)
            {
                throw new BusinessException("User:NotFound");
            }

            user.SetPassword();
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(long id, ChangeSysUserPasswordDto input)
        {
            var user = await _userManager.GetAsync(id);
            if (user == null)
            {
                throw new BusinessException("User:NotFound");
            }

            if (user.CheckPassword(input.OldPassword))
            {
                if (!input.NewPassword.IsNullOrWhiteSpace() && input.NewPassword != input.ConfirmPassword)
                {
                    throw new BusinessException("User:PasswordMismatch");
                }

                user.SetPassword(input.NewPassword);
                await _userManager.UpdateAsync(user);
                return true;
            }

            throw new BusinessException("User:OldPasswordIncorrect");
        }

        public async Task<SysUserDto> UpdateAsync(long id, CreateUpdateSysUserDto input)
        {
            var user = await _userManager.GetAsync(id);

            if (await _userManager.AnyAsync(u => u.UserCode == input.UserCode && u.Id != id))
            {
                throw new BusinessException("User:UserCodeExists").WithData("0", input.UserCode);
            }

            user.SetName(input.UserCode);
            user.SetContact(input.AliasName, input.PhoneNumber, input.Email);
            user.SetDepartment(input.DepartmentId);
            user.SetAdmin(input.IsAdmin);
            user.SetTeamAdmin(input.IsTeamAdmin);
            user.SetStatus(input.Status);
            user.SetRoleId(input.RoleId);

            await _userManager.UpdateAsync(user);
            return ObjectMapper.Map<SysUser, SysUserDto>(user);
        }

        public async Task<SysUserDto> UpdateSelfAsync(long id, UpdateSysUserSelfDto input)
        {
            var user = await _userManager.GetAsync(id);

            user.SetContact(input.AliasName, input.PhoneNumber, input.Email);

            await _userManager.UpdateAsync(user);
            return ObjectMapper.Map<SysUser, SysUserDto>(user);
        }

        public async Task<UserSelfInfoDto> GetSelfInfoAsync(long userId)
        {
            var userQuery = await _userManager.GetQueryableAsync();
            var user = await userQuery.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new BusinessException("User:NotFound");

            var userDto = ObjectMapper.Map<SysUser, SysUserDto>(user);

            List<SysMenus> menus;
            if (user.IsAdmin)
            {
                // 超级管理员获取所有可见菜单
                var queryable = await _menuRepository.GetQueryableAsync();
                menus = queryable
                    .Where(m => !m.IsDeleted && m.Visible)
                    .OrderBy(m => m.Sort)
                    .ToList();
            }
            else
            {
                // 非管理员通过用户自己的RoleId找到关联的菜单id，再找到对应的菜单
                var rolePowerQuery = await _rolePowerRepository.GetQueryableAsync();
                var menuIds = await rolePowerQuery
                    .Where(rp => rp.RoleId == user.RoleId)
                    .Select(rp => rp.MenuId)
                    .ToListAsync();

                var menuQuery = await _menuRepository.GetQueryableAsync();
                menus = await menuQuery
                    .Where(m => menuIds.Contains(m.Id) && !m.IsDeleted && m.Visible)
                    .OrderBy(m => m.Sort)
                    .ToListAsync();
            }

            var menuDtos = ObjectMapper.Map<List<SysMenus>, List<SysMenusDto>>(menus);

            // 提取所有权限标识
            var permissions = menus
                .Where(m => m.PermissionCode.Any())
                .SelectMany(m => m.PermissionCode.Split(','))
                .Distinct()
                .ToList();

            // 构建菜单树
            var menuTree = BuildMenuTree(menuDtos, _localizationContext.LanguageType);

            return new UserSelfInfoDto
            {
                UserInfo = userDto,
                Permissions = permissions,
                Menus = menuTree
            };
        }

        /// <summary>
        /// 将平面菜单列表构建为树形结构
        /// </summary>
        private static List<MenuTreeNodeDto> BuildMenuTree(List<SysMenusDto> menuList, GlobalLanguageType languageType)
        {
            var nodeDict = menuList.ToDictionary(m => m.Id, m => new MenuTreeNodeDto
            {
                // === 树构建基础字段 ===
                Id = m.Id,
                ParentId = m.ParentId,
                Sort = m.Sort,
                PermissionCode = new List<string>() { m.PermissionCode },
                MenuType = m.MenuType,
                Children = new List<MenuTreeNodeDto>(),

                // === Vue 路由字段（SysMenusMapDto） ===
                Path = m.Route,                              // 前端路由路径
                Component = m.ComponentPath,                  // 组件文件路径
                Name = m.ComponentName,                       // 路由名称（Vue Router name）
                Redirect = string.Empty,                      // 重定向（目录节点可在构建完成后补充）

                // === 路由元信息 ===
                Meta = new RouteMetaCustom
                {
                    Title = languageType == GlobalLanguageType.ZH ? m.Name : m.NameEn,       // 菜单显示名称(中英文)
                    Icon = m.Icon,                            // 菜单图标
                    Hidden = !m.Visible,                      // 不可见时隐藏路由
                    AlwaysShow = m.MenuType == SysMenuType.DIRECTORY  // 目录始终展开显示
                }
            });

            var tree = new List<MenuTreeNodeDto>();
            foreach (var node in nodeDict.Values.OrderBy(m => m.Sort))
            {
                if (node.ParentId == 0 || !nodeDict.TryGetValue(node.ParentId, out var parent))
                {
                    // 为根目录设置重定向到第一个子节点路径
                    if (node.MenuType == SysMenuType.DIRECTORY && node.Children.Count > 0)
                    {
                        node.Redirect = node.Children.First().Path;
                    }

                    tree.Add(node);
                }
                else
                {
                    parent.Children.Add(node);
                }
            }

            return tree;
        }
    }
}
