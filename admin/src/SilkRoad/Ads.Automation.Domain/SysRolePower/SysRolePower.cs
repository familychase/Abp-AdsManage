
namespace Ads.Automation.Domain.SysRolePower
{
    public class SysRolePower : AggregateRootEntity
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 菜单Id
        /// </summary>
        public long MenuId { get; set; }

        /// <summary>
        /// 关联的角色
        /// </summary>
        public SysRoles Role { get; set; } = null!;

        /// <summary>
        /// 关联的菜单
        /// </summary>
        public SysMenus Menu { get; set; } = null!;

        private SysRolePower() { }

        /// <summary>
        /// 创建角色菜单关联（Id 由数据库自增，无需显式赋值）
        /// </summary>
        public static SysRolePower Create(long roleId, long menuId)
        {
            return new SysRolePower(roleId, menuId);
        }

        internal SysRolePower(long roleId, long menuId)
            : base()
        {
            this.RoleId = roleId;
            this.MenuId = menuId;
        }
    }
}
