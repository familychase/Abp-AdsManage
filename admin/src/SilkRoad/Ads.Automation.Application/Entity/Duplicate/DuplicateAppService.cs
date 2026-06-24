using Ads.Automation.Domain.Duplicate;

namespace Ads.Automation.Application.Entity.Duplicate;

/// <summary>
/// 广告复制应用服务 —— API 路由层
/// 负责接收前端请求并委派给具体执行服务
/// </summary>
public class DuplicateAppService : ApplicationService, IDuplicateAppService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBaseRepository<AdsDuplicateLogging> _loggingRepository;
    private readonly IBaseRepository<AdsDuplicateDetail> _detailRepository;

    public DuplicateAppService(
        IServiceScopeFactory scopeFactory,
        IBaseRepository<AdsDuplicateLogging> loggingRepository,
        IBaseRepository<AdsDuplicateDetail> detailRepository)
    {
        _scopeFactory = scopeFactory;
        _loggingRepository = loggingRepository;
        _detailRepository = detailRepository;
    }

    /// <inheritdoc />
    public async Task<AdsDuplicateLoggingDto> InternalDuplicateAsync(InternalDuplicateInput input)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MetaDuplicateService>();
        var log = await service.InternalDuplicateAsync(input);
        return MapToDto(log);
    }

    /// <inheritdoc />
    public async Task<AdsDuplicateLoggingDto> ExternalDuplicateAsync(ExternalDuplicateInput input)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MetaDuplicateExternalService>();
        var log = await service.ExternalDuplicateAsync(input);
        return MapToDto(log);
    }

    /// <inheritdoc />
    public async Task<AdsDuplicateLoggingDto> SubmitCampaignAsync(CampaignSubmitInput input)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MetaDuplicateService>();
        var log = await service.SubmitCampaignAsync(input);
        return MapToDto(log);
    }

    /// <inheritdoc />
    public async Task<AdsDuplicateLoggingDto> SubmitAdSetAsync(AdSetSubmitInput input)
    {
        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<MetaDuplicateService>();
        var log = await service.SubmitAdSetAsync(input);
        return MapToDto(log);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<AdsDuplicateLoggingDto>> GetCampaignListAsync(GetDuplicateLoggingListInput input)
    {
        return await GetListByLevelAsync(input, AdObjectLevel.CAMPAIGN);
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<AdsDuplicateLoggingDto>> GetAdSetListAsync(GetDuplicateLoggingListInput input)
    {
        return await GetListByLevelAsync(input, AdObjectLevel.AD_SET);
    }

    private async Task<PagedResultDto<AdsDuplicateLoggingDto>> GetListByLevelAsync(
        GetDuplicateLoggingListInput input, AdObjectLevel level)
    {
        var query = await _loggingRepository.GetQueryableAsync();

        query = query.Where(x => x.AdObjectLevel == level);

        if (input.DuplicateSource.HasValue)
            query = query.Where(x => x.DuplicateSource == input.DuplicateSource.Value);

        if (input.State.HasValue)
            query = query.Where(x => x.State == input.State.Value);

        if (!string.IsNullOrWhiteSpace(input.AdObjectNo))
            query = query.Where(x => x.AdObjectNo.Contains(input.AdObjectNo));

        if (!string.IsNullOrWhiteSpace(input.AccountNo))
            query = query.Where(x => x.AccountNo == input.AccountNo);

        if (!string.IsNullOrWhiteSpace(input.DuplicateAccountNo))
            query = query.Where(x => x.DuplicateAccountNo == input.DuplicateAccountNo);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Select(x => new AdsDuplicateLoggingDto
            {
                Id = x.Id,
                DuplicateSource = x.DuplicateSource,
                ResourceId = x.ResourceId,
                IsInternal = x.IsInternal,
                AdObjectLevel = x.AdObjectLevel,
                AdObjectNo = x.AdObjectNo,
                AccountNo = x.AccountNo,
                DuplicateAccountNo = x.DuplicateAccountNo,
                PageNo = x.PageNo,
                State = x.State,
                DuplicateContent = x.DuplicateContent,
                ScheduleTime = x.ScheduleTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = x.EndTime.HasValue ? x.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                CreationTime = x.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                CreatorId = x.CreatorId,
                ExtendedData = x.ExtendedData,
                CopyNumber = x.CopyNumber,
                ErrorMessage = x.ErrorMessage
            })
            .ToListAsync();

        return new PagedResultDto<AdsDuplicateLoggingDto>(totalCount, items);
    }

    /// <summary>
    /// 将实体映射为 DTO（处理时间格式化）
    /// </summary>
    private static AdsDuplicateLoggingDto MapToDto(AdsDuplicateLogging entity)
    {
        return new AdsDuplicateLoggingDto
        {
            Id = entity.Id,
            DuplicateSource = entity.DuplicateSource,
            ResourceId = entity.ResourceId,
            IsInternal = entity.IsInternal,
            AdObjectLevel = entity.AdObjectLevel,
            AdObjectNo = entity.AdObjectNo,
            AccountNo = entity.AccountNo,
            DuplicateAccountNo = entity.DuplicateAccountNo,
            PageNo = entity.PageNo,
            State = entity.State,
            DuplicateContent = entity.DuplicateContent,
            ScheduleTime = entity.ScheduleTime.ToString("yyyy-MM-dd HH:mm:ss"),
            EndTime = entity.EndTime.HasValue ? entity.EndTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
            CreationTime = entity.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
            CreatorId = entity.CreatorId,
            ExtendedData = entity.ExtendedData,
            CopyNumber = entity.CopyNumber,
            ErrorMessage = entity.ErrorMessage
        };
    }

    /// <inheritdoc />
    public async Task<List<AdsDuplicateDetailDto>> GetDetailListAsync(long logId)
    {
        var query = await _detailRepository.GetQueryableAsync();

        var details = await query
            .Where(x => x.LogId == logId)
            .OrderBy(x => x.Index)
            .Select(x => ObjectMapper.Map<AdsDuplicateDetail, AdsDuplicateDetailDto>(x))
            .ToListAsync();

        return details;
    }
}
