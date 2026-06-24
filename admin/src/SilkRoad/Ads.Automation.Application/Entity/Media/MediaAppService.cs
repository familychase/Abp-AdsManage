using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Media;

/// <summary>
/// 媒体实时查询应用服务
/// 封装所有需要实时调用 Meta API 的操作
/// </summary>
public class MediaAppService : ApplicationService, IMediaAppService
{
    private readonly IMediaAuthService _mediaAuth;
    private readonly MetaApiRetryPolicy _retry;

    public MediaAppService(
        IMediaAuthService mediaAuth,
        MetaApiRetryPolicy retry)
    {
        _mediaAuth = mediaAuth;
        _retry = retry;
    }

    /// <inheritdoc />
    public async Task<CampaignDetailDto> GetCampaignDetailAsync(CampaignDetailInput input)
    {
        var identity = await _mediaAuth.GetFromAnyChannelAsync();

        const string queryFields = "id,name,status,account_id,objective,daily_budget,lifetime_budget";
        var campaign = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCampaignAsync(identity, input.CampaignNo, queryFields),
            "获取广告系列详情",
            input.AccountNo);

        if (campaign == null)
            throw new BusinessException(string.Format(L["Business:CampaignAccessDenied"], input.CampaignNo));

        var actualAccountId = campaign.account_id ?? string.Empty;
        var isMatched = IsSameAccount(actualAccountId, input.AccountNo);
        var mismatchMessage = isMatched
            ? null
            : string.Format(L["Business:AccountMismatch"], actualAccountId, input.AccountNo);

        return new CampaignDetailDto
        {
            CampaignNo = campaign.id ?? input.CampaignNo,
            CampaignName = campaign.name ?? string.Empty,
            Status = campaign.status ?? string.Empty,
            ActualAccountNo = actualAccountId,
            IsAccountMatched = isMatched,
            AccountMismatchMessage = mismatchMessage,
            Objective = campaign.objective ?? string.Empty,
            DailyBudget = ParseBudget(campaign.daily_budget),
            LifetimeBudget = ParseBudget(campaign.lifetime_budget)
        };
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<MediaPageDto>> GetPagesByAccountAsync(GetPagesByAccountInput input)
    {
        var identity = await _mediaAuth.GetFromAnyChannelAsync();

        var accountNo = NormalizeAccountNo(input.AccountNo);
        var result = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetPromotePagesAsync(identity, $"act_{accountNo}", 500, MetaConst.PageFields),
            "获取公共主页列表",
            accountNo);

        if (result?.data == null || result.data.Count == 0)
            return new PagedResultDto<MediaPageDto>();

        var list = result.data
            .Where(p => p.id != null)
            .Select(p => new MediaPageDto
            {
                PageNo = p.id!,
                PageName = p.name ?? string.Empty,
                Category = p.category
            })
            .ToList();

        return new PagedResultDto<MediaPageDto>(list.Count, list);
    }

    /// <inheritdoc />
    public async Task<List<BatchDeleteCampaignResultDto>> BatchDeleteCampaignsAsync(BatchDeleteCampaignsInput input)
    {
        var identity = await _mediaAuth.GetByAccountNoAsync(input.AccountNo);
        var results = new List<BatchDeleteCampaignResultDto>();

        foreach (var campaignNo in input.CampaignNos)
        {
            if (string.IsNullOrWhiteSpace(campaignNo))
                continue;

            try
            {
                await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdManagementDeleteAsync(identity, campaignNo),
                    "删除广告系列",
                    input.AccountNo);

                results.Add(new BatchDeleteCampaignResultDto
                {
                    CampaignNo = campaignNo,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                results.Add(new BatchDeleteCampaignResultDto
                {
                    CampaignNo = campaignNo,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<List<BatchDeleteAdSetResultDto>> BatchDeleteAdSetsAsync(BatchDeleteAdSetsInput input)
    {
        var identity = await _mediaAuth.GetByAccountNoAsync(input.AccountNo);
        var results = new List<BatchDeleteAdSetResultDto>();

        foreach (var adSetNo in input.AdSetNos)
        {
            if (string.IsNullOrWhiteSpace(adSetNo))
                continue;

            try
            {
                await _retry.ExecuteAsync(
                    () => MetaOpenApi.AdManagementDeleteAsync(identity, adSetNo),
                    "删除广告组",
                    input.AccountNo);

                results.Add(new BatchDeleteAdSetResultDto
                {
                    AdSetNo = adSetNo,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                results.Add(new BatchDeleteAdSetResultDto
                {
                    AdSetNo = adSetNo,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        return results;
    }

    /// <inheritdoc />
    public async Task<List<MediaRegionDto>> GetRegionListAsync(GetRegionListInput input)
    {
        var identity = await _mediaAuth.GetFromAnyChannelAsync();

        var locationTypes = input.LocationTypes is { Count: > 0 }
            ? input.LocationTypes.ToArray()
            : new[] { "region", "city" };

        var result = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCountryAsync(identity, input.Keyword, locationTypes, input.Limit, input.Locale),
            "获取区域/城市列表",
            null);

        if (result?.data == null || result.data.Count == 0)
            return new List<MediaRegionDto>();

        return result.data.Select(item => new MediaRegionDto
        {
            Key = item.key ?? string.Empty,
            Name = item.name ?? string.Empty,
            Type = item.type,
            CountryCode = item.country_code,
            CountryName = item.country_name,
            Region = item.region,
            RegionId = item.region_id,
            SupportsRegion = item.supports_region,
            SupportsCity = item.supports_city,
            CountryCodes = item.country_codes
        }).ToList();
    }

    private static decimal? ParseBudget(string? budgetCents)
    {
        if (string.IsNullOrEmpty(budgetCents) || !decimal.TryParse(budgetCents, out var value))
            return null;
        return value / 100M;
    }

    private static bool IsSameAccount(string metaAccountId, string inputAccountNo)
    {
        if (string.IsNullOrWhiteSpace(metaAccountId) || string.IsNullOrWhiteSpace(inputAccountNo))
            return false;
        return NormalizeAccountNo(metaAccountId) == NormalizeAccountNo(inputAccountNo);
    }

    private static string NormalizeAccountNo(string accountNo)
    {
        var normalized = accountNo.Trim();
        if (normalized.StartsWith("act_", StringComparison.OrdinalIgnoreCase))
            normalized = normalized[4..];
        return normalized;
    }
}
