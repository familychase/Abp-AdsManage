using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts;

/// <summary>
/// 自定义分页请求 DTO，支持前端传入 page / page_size 字段
/// 自动映射为 ABP 标准的 SkipCount / MaxResultCount
/// </summary>
public class BasePagedAndSortedRequestDto : PagedAndSortedResultRequestDto
{
    private int _page = 1;
    private int _pageSize = 10;

    /// <summary>
    /// 当前页码（从 1 开始），对应 SkipCount = (Page - 1) * PageSize
    /// </summary>
    public int Page
    {
        get => _page;
        set => _page = value > 0 ? value : 1;
    }

    /// <summary>
    /// 每页条数，对应 MaxResultCount
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 0 ? value : 10;
    }

    /// <summary>
    /// 跳过条数，由 Page 和 PageSize 自动计算
    /// </summary>
    [JsonIgnore]
    public override int SkipCount => (Page - 1) * PageSize;

    /// <summary>
    /// 每页最大条数，由 PageSize 控制
    /// </summary>
    [JsonIgnore]
    public override int MaxResultCount => PageSize;
}
