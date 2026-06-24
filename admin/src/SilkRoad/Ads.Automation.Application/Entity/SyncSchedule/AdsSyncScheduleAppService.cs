
using Ads.Automation.Application.Contracts.Entity.SyncSchedule;
using Ads.Automation.Domain.Shared.Localization;
using Ads.Automation.Domain.SyncSchedule;
using Microsoft.Extensions.Localization;

namespace Ads.Automation.Application.Entity.SyncSchedule;

/// <summary>
/// 同步调度计划应用服务实现
/// </summary>
public class AdsSyncScheduleAppService : ApplicationService, IAdsSyncScheduleAppService
{
    private readonly IBaseRepository<AdsSyncSchedule> _scheduleRepository;
    private readonly IStringLocalizer<AdsAutomationResource> _loc;

    public AdsSyncScheduleAppService(
        IBaseRepository<AdsSyncSchedule> scheduleRepository,
        IStringLocalizer<AdsAutomationResource> loc)
    {
        _scheduleRepository = scheduleRepository;
        _loc = loc;
    }

    public async Task<AdsSyncScheduleDto> GetAsync(long id)
    {
        var schedule = await _scheduleRepository.GetAsync(s => s.Id == id);
        var dto = ObjectMapper.Map<AdsSyncSchedule, AdsSyncScheduleDto>(schedule);
        dto.JobNameDisplay = MapJobName(dto.JobName, dto.LinkDate);
        return dto;
    }

    public async Task<PagedResultDto<AdsSyncScheduleDto>> GetListAsync(GetAdsSyncScheduleListInput input)
    {
        var query = await _scheduleRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(s =>
                s.JobName.Contains(input.FilterText) ||
                s.ResourceId.Contains(input.FilterText));
        }

        if (input.ActionType.HasValue)
        {
            query = query.Where(s => s.ActionType == input.ActionType.Value);
        }

        if (input.Platform.HasValue)
        {
            query = query.Where(s => s.Platform == input.Platform.Value);
        }

        if (input.ResourceType.HasValue)
        {
            query = query.Where(s => s.ResourceType == input.ResourceType.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.ResourceId))
        {
            query = query.Where(s => s.ResourceId == input.ResourceId);
        }

        var totalCount = query.Count();

        query = query
            .OrderBy(s => s.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = ObjectMapper.Map<List<AdsSyncSchedule>, List<AdsSyncScheduleDto>>(query.ToList());

        foreach (var item in items)
        {
            item.JobNameDisplay = MapJobName(item.JobName, item.LinkDate);
        }

        return new PagedResultDto<AdsSyncScheduleDto>(totalCount, items);
    }

    public async Task<AdsSyncScheduleDto> CreateAsync(CreateUpdateAdsSyncScheduleDto input)
    {
        var schedule = AdsSyncSchedule.Create(
            input.ActionType,
            input.ResourceId,
            input.ResourceType,
            input.Platform,
            input.JobName,
            input.ExtendingData,
            input.Level,
            input.IsAudience,
            input.LinkDate,
            input.NextPublishTime);

        await _scheduleRepository.InsertAsync(schedule);
        var dto = ObjectMapper.Map<AdsSyncSchedule, AdsSyncScheduleDto>(schedule);
        dto.JobNameDisplay = MapJobName(dto.JobName, dto.LinkDate);
        return dto;
    }

    public async Task<AdsSyncScheduleDto> UpdateAsync(long id, CreateUpdateAdsSyncScheduleDto input)
    {
        var schedule = await _scheduleRepository.GetAsync(s => s.Id == id);

        schedule.SetActionType(input.ActionType);
        schedule.SetResourceId(input.ResourceId);
        schedule.SetResourceType(input.ResourceType);
        schedule.SetPlatform(input.Platform);
        schedule.SetJobName(input.JobName);
        schedule.SetExtendingData(input.ExtendingData);
        schedule.SetLevel(input.Level);
        schedule.SetIsAudience(input.IsAudience);
        schedule.SetLinkDate(input.LinkDate);
        schedule.SetNextPublishTime(input.NextPublishTime);

        await _scheduleRepository.UpdateAsync(schedule);
        var dto = ObjectMapper.Map<AdsSyncSchedule, AdsSyncScheduleDto>(schedule);
        dto.JobNameDisplay = MapJobName(dto.JobName, dto.LinkDate);
        return dto;
    }

    public async Task DeleteAsync(long id)
    {
        await _scheduleRepository.DeleteAsync(s => s.Id == id, true);
    }

    private string MapJobName(string jobName, string? linkDate = null)
    {
        // 同步报表区分日期类型
        if (jobName == "SyncAdReportJobArgs")
            return MapReportJobName(linkDate);

        return jobName switch
        {
            "SyncAdAccountJobArgs"  => _loc["SyncSchedule:JobName:AdAccount"],
            "SyncAdPageJobArgs"     => _loc["SyncSchedule:JobName:AdPage"],
            "SyncAdPixelJobArgs"    => _loc["SyncSchedule:JobName:AdPixel"],
            "SyncAdCampaignJobArgs" => _loc["SyncSchedule:JobName:AdCampaign"],
            "SyncAdAudienceJobArgs" => _loc["SyncSchedule:JobName:AdAudience"],
            _                       => jobName
        };
    }

    private string MapReportJobName(string? linkDate)
    {
        return linkDate?.ToLowerInvariant() switch
        {
            "yesterday" => _loc["SyncSchedule:JobName:AdReportYesterday"],
            "today"     => _loc["SyncSchedule:JobName:AdReportToday"],
            _ when !string.IsNullOrWhiteSpace(linkDate)
                => $"{_loc["SyncSchedule:JobName:AdReport"]}({linkDate})",
            _ => _loc["SyncSchedule:JobName:AdReport"]
        };
    }
}
