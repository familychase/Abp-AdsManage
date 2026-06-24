namespace Ads.Automation.Application.Contracts.Entity.Departments;

/// <summary>
/// 获取部门列表 Input
/// </summary>
public class GetSysDepartmentListInput : BasePagedAndSortedRequestDto
{
    /// <summary>
    /// 部门名称过滤
    /// </summary>
    public string? FilterText { get; set; }

    /// <summary>
    /// 父部门 ID 过滤
    /// </summary>
    public long? ParentId { get; set; }
}
