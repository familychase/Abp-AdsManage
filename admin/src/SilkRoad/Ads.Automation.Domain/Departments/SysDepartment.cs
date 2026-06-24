namespace Ads.Automation.Domain.Departments;

public class SysDepartment : AggregateRootEntity, IHasCreationTimeEntity, IHasModificationTimeEntity, ISoftDeleteEntity
{
    /// <summary>
    /// 父部门
    /// </summary>
    public long ParentId { get; private set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; private set; } = string.Empty;

    /// <summary>
    /// 部门别名
    /// </summary>
    public string? AliasName { get; private set; } = string.Empty;

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; private set; }

    /// <summary>
    /// 树路径
    /// </summary>
    public string? Path { get; private set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; private set; } = string.Empty;

    public long CreatorId { get; set; }

    public DateTime CreationTime { get; set; } = DateTime.Now;

    public long? LastModifierId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public long? DeleterId { get; set; }

    public DateTime? DeletionTime { get; set; }

    public bool IsDeleted { get; set; }

    private SysDepartment() { }

    public static SysDepartment Create(long parentId, string deptName, int sort = 0, string? aliasName = null, string? path = null, string? remark = null)
    {
        return new SysDepartment(IdGenerator.GetNextId(), parentId, deptName, sort, aliasName, path, remark);
    }

    internal SysDepartment(long id, long parentId, string deptName, int sort = 0, string? aliasName = null, string? path = null,  string? remark = null)
        : base(id)
    {
        ParentId = parentId;
        DeptName = deptName ?? string.Empty;
        Sort = sort;
        AliasName = aliasName ?? string.Empty;
        Path = path ?? string.Empty;
        Remark = remark ?? string.Empty;
    }

    public void SetName(string name)
    {
        DeptName = Check.NotNullOrWhiteSpace(name, nameof(name), 128);
    }

    public void SetAlias(string? alias)
    {
        AliasName = Check.Length(alias, nameof(alias), 64);
    }

    public void SetPath(string? path)
    {
        Path = Check.Length(path, nameof(path), 512);
    }

    public void SetRemark(string? remark)
    {
        Remark = Check.Length(remark, nameof(remark), 500);
    }

    public void SetParent(long parentId)
    {
        ParentId = parentId;
    }
}
