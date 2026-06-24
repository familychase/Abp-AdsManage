using SysRolePowerEntity = Ads.Automation.Domain.SysRolePower.SysRolePower;

namespace Ads.Automation.Domain.Roles;

public class SysRoles : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity, ISoftDeleteEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; private set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; }

    /// <summary>
    /// 角色关联的菜单权限（多对多中间表）
    /// </summary>
    public ICollection<SysRolePowerEntity> RolePowers { get; set; } = new List<SysRolePowerEntity>();

    public long CreatorId { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;

    public long? LastModifierId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? DeleterId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public bool IsDeleted { get; set; }

    private SysRoles() { }

    public static SysRoles Create( string name, int sort, string? remark = null)
    {
        return new SysRoles(IdGenerator.GetNextId(), name, sort, remark);
    }

    internal SysRoles(long id, string name, int sort, string? remark = null)
        : base(id)
    {
        Name = name;
        Sort = sort;
        Remark = remark;
    }

    // ===== 更新方法 =====

    public void SetName(string name) => Name = name;
    public void SetSort(int sort) => Sort = sort;
    public void SetRemark(string? remark) => Remark = remark;
}