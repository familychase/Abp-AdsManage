
namespace Ads.Automation.Application.Entity.Account;

/// <summary>
/// 广告账户应用服务实现
/// </summary>
public class AdsAccountAppService : ApplicationService, IAdsAccountAppService
{
    private readonly IBaseRepository<AdsAccount> _accountRepository;
    private readonly IBaseRepository<AdsChannelAdAccount> _channelAdAccountRepository;

    public AdsAccountAppService(
        IBaseRepository<AdsAccount> accountRepository,
        IBaseRepository<AdsChannelAdAccount> channelAdAccountRepository)
    {
        _accountRepository = accountRepository;
        _channelAdAccountRepository = channelAdAccountRepository;
    }

    public async Task<AdsAccountDto> GetAsync(long id)
    {
        var account = await _accountRepository.GetAsync(a => a.Id == id);
        return ObjectMapper.Map<AdsAccount, AdsAccountDto>(account);
    }

    public async Task<PagedResultDto<AdsAccountDto>> GetListAsync(GetAdsAccountListInput input)
    {
        var query = (await _accountRepository.GetQueryableAsync()).AsNoTracking();

        // 授权渠道ID过滤：通过 ads_channel_adaccounts 关联表筛选
        if (input.ChannelId.HasValue)
        {
            var relationQuery = await _channelAdAccountRepository.GetQueryableAsync();
            var accountNos = relationQuery
                .Where(r => r.ChannelId == input.ChannelId.Value)
                .Select(r => r.AccountNo);
            query = query.Where(a => accountNos.Contains(a.AccountNo));
        }

        // 编号筛选（独立字段，精确或包含匹配）
        if (!string.IsNullOrWhiteSpace(input.AccountNo))
        {
            query = query.Where(a => a.AccountNo != null && a.AccountNo.Contains(input.AccountNo));
        }

        // 名字筛选（独立字段，模糊匹配）
        if (!string.IsNullOrWhiteSpace(input.AccountName))
        {
            query = query.Where(a => a.AccountName != null && a.AccountName.Contains(input.AccountName));
        }

        // FilterText：同时模糊匹配编号和名称（向后兼容）
        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(a =>
                (a.AccountName != null && a.AccountName.Contains(input.FilterText)) ||
                (a.AccountNo != null && a.AccountNo.Contains(input.FilterText)));
        }

        // 平台过滤
        if (input.Platform.HasValue)
        {
            query = query.Where(a => a.Platform == input.Platform.Value);
        }

        // 账户状态过滤
        if (input.AccountState.HasValue)
        {
            query = query.Where(a => a.AccountState == input.AccountState.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => (int)a.AccountState)
            .ThenByDescending(a => a.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .Select(a => new AdsAccountDto
            {
                Id = a.Id,
                AccountNo = a.AccountNo,
                AccountName = a.AccountName,
                AccountState = a.AccountState,
                MediaState = a.MediaState,
                Balance = a.Balance,
                Timezone = a.Timezone,
                UtcTimezoneOffset = a.UtcTimezoneOffset,
                CreationTime = a.CreationTime,
                LastModificationTime = a.LastModificationTime,
                Platform = a.Platform,
                OwnerId = a.OwnerId,
                OwnerTeamId = a.OwnerTeamId,
                IsManager = a.IsManager,
                Currency = a.Currency,
                IsLimit = a.IsLimit,
                MediaDisableReason = a.MediaDisableReason,
                MediaCreatedTime = a.MediaCreatedTime,
                AccountRunningTime = a.AccountRunningTime,
            })
            .ToListAsync();

        return new PagedResultDto<AdsAccountDto>(totalCount, items);
    }

    public async Task<AdsAccountDto> CreateAsync(CreateUpdateAdsAccountDto input)
    {
        var account = AdsAccount.Create(
            input.AccountNo,
            input.AccountName,
            input.AccountState,
            input.MediaState,
            input.Balance,
            input.Timezone,
            input.UtcTimezoneOffset,
            input.Platform,
            input.OwnerId,
            input.OwnerTeamId,
            input.IsManager,
            input.Currency,
            input.IsLimit,
            input.MediaDisableReason,
            input.MediaCreatedTime,
            input.AccountRunningTime);

        await _accountRepository.InsertAsync(account);
        return ObjectMapper.Map<AdsAccount, AdsAccountDto>(account);
    }

    public async Task<AdsAccountDto> UpdateAsync(long id, CreateUpdateAdsAccountDto input)
    {
        var account = await _accountRepository.GetAsync(a => a.Id == id);

        account.SetAccountNo(input.AccountNo);
        account.SetAccountName(input.AccountName);
        account.SetAccountState(input.AccountState);
        account.SetMediaState(input.MediaState);
        account.SetBalance(input.Balance);
        account.SetTimezone(input.Timezone);
        account.SetUtcTimezoneOffset(input.UtcTimezoneOffset);
        account.SetPlatform(input.Platform);
        account.SetOwnerId(input.OwnerId);
        account.SetOwnerTeamId(input.OwnerTeamId);
        account.SetIsManager(input.IsManager);
        account.SetCurrency(input.Currency);
        account.SetIsLimit(input.IsLimit);
        account.SetMediaDisableReason(input.MediaDisableReason);
        account.SetMediaCreatedTime(input.MediaCreatedTime);
        account.SetAccountRunningTime(input.AccountRunningTime);

        await _accountRepository.UpdateAsync(account);
        return ObjectMapper.Map<AdsAccount, AdsAccountDto>(account);
    }

    public async Task DeleteAsync(long id)
    {
        await _accountRepository.DeleteAsync(a => a.Id == id, true);
    }
}
