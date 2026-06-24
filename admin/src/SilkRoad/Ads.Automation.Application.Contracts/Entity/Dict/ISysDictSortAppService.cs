namespace Ads.Automation.Application.Contracts.Entity.Dict;

/// <summary>
/// 系统字典类 AppService 接口
/// </summary>
public interface ISysDictSortAppService : IApplicationService
{
    /// <summary>
    /// 获取字典类
    /// </summary>
    Task<SysDictSortDto> GetAsync(long id);

    /// <summary>
    /// 获取字典类列表
    /// </summary>
    Task<PagedResultDto<SysDictSortDto>> GetListAsync(GetSysDictSortListInput input);

    /// <summary>
    /// 创建字典类
    /// </summary>
    Task<SysDictSortDto> CreateAsync(CreateUpdateSysDictSortDto input);

    /// <summary>
    /// 修改字典类
    /// </summary>
    Task<SysDictSortDto> UpdateAsync(long id, CreateUpdateSysDictSortDto input);

    /// <summary>
    /// 删除字典类
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 批量创建字典类 + 字典项
    /// </summary>
    Task AddSortWithItemsAsync(CreateUpdateDictSortWithItemsDto input);

    /// <summary>
    /// 获取字典最后更新时间
    /// </summary>
    Task<DateTime?> GetLastUpdateTimeAsync();
}
