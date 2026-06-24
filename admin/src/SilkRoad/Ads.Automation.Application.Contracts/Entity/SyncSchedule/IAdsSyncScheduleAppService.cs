namespace Ads.Automation.Application.Contracts.Entity.SyncSchedule;

/// <summary>
/// 同步调度计划应用服务接口
/// </summary>
public interface IAdsSyncScheduleAppService : IApplicationService
{
    /// <summary>
    /// 获取调度计划
    /// </summary>
    Task<AdsSyncScheduleDto> GetAsync(long id);

    /// <summary>
    /// 获取调度计划列表
    /// </summary>
    Task<PagedResultDto<AdsSyncScheduleDto>> GetListAsync(GetAdsSyncScheduleListInput input);

    /// <summary>
    /// 创建调度计划
    /// </summary>
    Task<AdsSyncScheduleDto> CreateAsync(CreateUpdateAdsSyncScheduleDto input);

    /// <summary>
    /// 修改调度计划
    /// </summary>
    Task<AdsSyncScheduleDto> UpdateAsync(long id, CreateUpdateAdsSyncScheduleDto input);

    /// <summary>
    /// 删除调度计划
    /// </summary>
    Task DeleteAsync(long id);
}
