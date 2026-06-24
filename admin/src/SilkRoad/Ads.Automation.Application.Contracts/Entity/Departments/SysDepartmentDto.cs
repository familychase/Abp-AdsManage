namespace Ads.Automation.Application.Contracts.Entity.Departments;

/// <summary>
/// 部门信息 Dto
/// </summary>
public class SysDepartmentDto
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 父部门 ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 父部门名称
    /// </summary>
    public string ParentName { get; set; } = string.Empty;

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; } = default!;

    /// <summary>
    /// 部门别名
    /// </summary>
    public string? AliasName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreationTime { get; set; } = string.Empty;

    /// <summary>
    /// 子部门列表（树形结构）
    /// </summary>
    public List<SysDepartmentDto> Children { get; set; } = new();
}
