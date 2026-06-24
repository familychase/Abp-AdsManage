using Volo.Abp.Validation;

namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典项 AppService 接口
/// </summary>
public interface ISysDictItemAppService : IApplicationService
{
    /// <summary>
    /// 获取字典项
    /// </summary>
    Task<SysDictItemDetailDto> GetAsync(long id);

    /// <summary>
    /// 获取字典项列表
    /// </summary>
    Task<PagedResultDto<SysDictItemDetailDto>> GetListAsync(GetSysDictItemListInput input);

    /// <summary>
    /// 获取字典项树形结构
    /// </summary>
    Task<List<SysDictItemTreeNodeDto>> GetTreeAsync(long dictSortId);

    /// <summary>
    /// 创建字典项
    /// </summary>
    Task<SysDictItemDetailDto> CreateAsync(CreateUpdateSysDictItemDto input);

    /// <summary>
    /// 修改字典项
    /// </summary>
    Task<SysDictItemDetailDto> UpdateAsync(long id, CreateUpdateSysDictItemDto input);

    /// <summary>
    /// 删除字典项
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 批量获取多个字典类编码对应的字典项（web_list 接口用）
    /// 传 null 或空集合时查询全部字典
    /// </summary>
    [DisableValidation]
    Task<Dictionary<string, List<SysDictItemDto>>> GetItemsBySortCodesAsync(List<string>? sortCodes);

    /// <summary>
    /// 按字典类ID或编码获取字典项列表（非分页，下拉框用）
    /// </summary>
    Task<List<SysDictItemDto>> GetItemsBySortAsync(long? sortId, string? sortCode);
}
