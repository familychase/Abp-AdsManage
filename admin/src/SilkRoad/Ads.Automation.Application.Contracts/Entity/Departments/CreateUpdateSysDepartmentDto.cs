namespace Ads.Automation.Application.Contracts.Entity.Departments;

/// <summary>
/// 创建/修改部门信息 Dto
/// </summary>
public class CreateUpdateSysDepartmentDto
{
    /// <summary>
    /// 父部门 ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [Required]
    [StringLength(128)]
    public string DeptName { get; set; } = string.Empty;

    /// <summary>
    /// 部门别名
    /// </summary>
    [StringLength(64)]
    public string? AliasName { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 树路径
    /// </summary>
    [StringLength(512)]
    public string? Path { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500)]
    public string? Remark { get; set; }
}
