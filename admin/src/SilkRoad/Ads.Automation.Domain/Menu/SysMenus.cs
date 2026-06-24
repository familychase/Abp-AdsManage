namespace Ads.Automation.Domain.Menu
{
    public class SysMenus : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity, ISoftDeleteEntity
    {
        /// <summary>
        /// 父菜单
        /// </summary>
        public long ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜单名称-英文
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// 前端路由
        /// </summary>
        public string Route { get; set; } = string.Empty;

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// 类型（目录、菜单、按钮）
        /// </summary>
        public SysMenuType MenuType { get; set; }

        /// <summary>
        /// 权限标识
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 前端组件名称
        /// </summary>
        public string ComponentName { get; set; } = string.Empty;

        /// <summary>
        /// 前端组件路径
        /// </summary>
        public string ComponentPath { get; set; } = string.Empty;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public long CreatorId { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public long? LastModifierId { get; set; }

        /// <summary>
        /// 软删除
        /// </summary>
        public bool IsDeleted { get; set; }

        [NotMapped]
        public long? DeleterId { get; set; }

        [NotMapped]
        public DateTime? DeletionTime { get; set; }


        private SysMenus()
        {
        }

        public static SysMenus Create(string name, string nameEn, long parentId, string route, string icon, SysMenuType menuType,
            string permissionCode, string componentName, string componentPath,
            int sort, bool visible, string remark, DateTime creationTime, long creatorId,
            DateTime? lastModificationTime, long? lastModifierId)
        {
            return new SysMenus(IdGenerator.GetNextId(), parentId, name, nameEn, route, icon, menuType, permissionCode,
                componentName, componentPath, sort, visible, remark, creationTime, creatorId,
                lastModificationTime, lastModifierId);
        }

        internal SysMenus(long id, long parentId, string name, string nameEn, string route, string icon, SysMenuType menuType,
            string permissionCode, string componentName, string componentPath,
            int sort, bool visible, string remark, DateTime creationTime, long creatorId,
            DateTime? lastModificationTime, long? lastModifierId)
            : base(id)
        {
            this.ParentId = parentId;
            this.Name = name;
            this.NameEn = NameEn;
            this.Route = route;
            this.Icon = icon;
            this.MenuType = menuType;
            this.PermissionCode = permissionCode;
            this.ComponentName = componentName;
            this.ComponentPath = componentPath;
            this.Sort = sort;
            this.Visible = visible;
            this.Remark = remark;
            this.CreationTime = creationTime;
            this.CreatorId = creatorId;
            this.LastModificationTime = lastModificationTime;
            this.LastModifierId = lastModifierId;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public enum SysMenuType
    {
        /// <summary>
        /// 表示目录（文件夹）。
        /// </summary>
        DIRECTORY = 1,

        /// <summary>
        /// 表示菜单项（可以点击的菜单）。
        /// </summary>
        MENU = 2,

        /// <summary>
        /// 表示按钮。
        /// </summary>
        BUTTON = 3
    }
}