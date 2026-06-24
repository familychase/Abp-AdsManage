using Ads.Automation.Domain.Ads;
using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Campaign;

/// <summary>
/// 广告系列应用服务
/// 获取广告系列列表/详情
/// </summary>
public class CampaignAppService : ApplicationService, ICampaignAppService
{
    private readonly IBaseRepository<AdCampaignEntity> _campaignRepository;
    private readonly IBaseRepository<AdsAccount> _accountRepository;
    private readonly MetaApiRetryPolicy _retry;
    private readonly IMediaAuthService _mediaAuth;

    public CampaignAppService(
        IBaseRepository<AdCampaignEntity> campaignRepository,
        IBaseRepository<AdsAccount> accountRepository,
        MetaApiRetryPolicy retry,
        IMediaAuthService mediaAuth)
    {
        _campaignRepository = campaignRepository;
        _accountRepository = accountRepository;
        _retry = retry;
        _mediaAuth = mediaAuth;
    }

    /// <inheritdoc />
    public async Task<CampaignDetailDto> GetDetailAsync(CampaignDetailInput input)
    {
        // 1. 获取渠道身份
        var identity = await _mediaAuth.GetFromAnyChannelAsync();

        // 2. 调用 Meta API 获取广告系列详情
        const string queryFields = "id,name,status,account_id,objective,daily_budget,lifetime_budget";
        var campaign = await _retry.ExecuteAsync(
            () => MetaOpenApi.GetCampaignAsync(identity, input.CampaignNo, queryFields),
            "获取广告系列详情",
            input.AccountNo);

        if (campaign == null)
            throw new BusinessException(string.Format(L["Business:CampaignAccessDenied"], input.CampaignNo));

        // 3. Verify account match
        var actualAccountId = campaign.account_id ?? string.Empty;
        var isMatched = IsSameAccount(actualAccountId, input.AccountNo);
        var mismatchMessage = isMatched
            ? null
            : string.Format(L["Business:AccountMismatch"], actualAccountId, input.AccountNo);

        // 4. 构建返回结果
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

    /// <summary>
    /// 将 Meta API 返回的美分预算字符串解析为美元金额（÷100）
    /// </summary>
    private static decimal? ParseBudget(string? budgetCents)
    {
        if (string.IsNullOrEmpty(budgetCents) || !decimal.TryParse(budgetCents, out var value))
            return null;
        return value / 100M;
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<CampaignListItemDto>> GetListAsync(GetCampaignListInput input)
    {
        var campaignQuery = await _campaignRepository.GetQueryableAsync();
        var accountQuery = await _accountRepository.GetQueryableAsync();

        // JOIN: campaign → account（获取平台信息）
        var query = from c in campaignQuery
                    join a in accountQuery on c.AccountId equals a.Id
                    select new { Campaign = c, Account = a };

        // AccountIds 过滤
        if (!input.AccountIds.IsNullOrEmpty() && input.AccountIds!.Any())
            query = query.Where(x => input.AccountIds!.Contains(x.Campaign.AccountId));

        // Platform 过滤
        if (input.Platform.HasValue)
            query = query.Where(x => x.Account.Platform == input.Platform.Value);

        // CampaignName 模糊搜索
        if (!string.IsNullOrWhiteSpace(input.CampaignName))
            query = query.Where(x => x.Campaign.CampaignName.Contains(input.CampaignName));

        // CampaignNo 精确筛选
        if (!string.IsNullOrWhiteSpace(input.CampaignNo))
            query = query.Where(x => x.Campaign.CampaignNo == input.CampaignNo);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.Campaign.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Select(x => new CampaignListItemDto
            {
                CampaignId = x.Campaign.Id,
                CampaignNo = x.Campaign.CampaignNo,
                CampaignName = x.Campaign.CampaignName,
                MediaState = x.Campaign.MediaState,
                BudgetType = x.Campaign.BudgetType,
                Budget = x.Campaign.Budget,
                Objective = x.Campaign.Objective,
                AccountNo = x.Campaign.AccountNo,
                MediaCreateTime = x.Campaign.MediaCreateTime,
                CreationTime = x.Campaign.CreationTime,
            })
            .ToListAsync();

        return new PagedResultDto<CampaignListItemDto>(totalCount, items);
    }

    /// <summary>
    /// 获取第一个可用渠道的 AccessIdentity（通过授权网关自动解密 Token）
    /// </summary>
    /// <summary>
    /// 比较两个账户编号是否相同（忽略 act_ 前缀差异）
    /// </summary>
    private static bool IsSameAccount(string metaAccountId, string inputAccountNo)
    {
        if (string.IsNullOrWhiteSpace(metaAccountId) || string.IsNullOrWhiteSpace(inputAccountNo))
            return false;

        return NormalizeAccountNo(metaAccountId) == NormalizeAccountNo(inputAccountNo);
    }

    /// <summary>
    /// 标准化账户编号：去掉 "act_" 前缀
    /// </summary>
    private static string NormalizeAccountNo(string accountNo)
    {
        var normalized = accountNo.Trim();
        if (normalized.StartsWith("act_", StringComparison.OrdinalIgnoreCase))
            normalized = normalized[4..];
        return normalized;
    }
}
