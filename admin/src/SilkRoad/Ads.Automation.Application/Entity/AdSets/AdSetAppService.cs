using Ads.Automation.Application.Contracts.Entity.AdSets;
using Ads.Automation.Domain.Account;
using Ads.Automation.Domain.Ads;

namespace Ads.Automation.Application.Entity.AdSets
{
    /// <summary>
    /// 广告组服务
    /// </summary>
    public class AdSetAppService : ApplicationService, IAdSetAppService
    {
        private readonly IBaseRepository<AdSetEntity> _adSetRepository;
        private readonly IBaseRepository<AdsAccount> _accountRepository;

        public AdSetAppService(
            IBaseRepository<AdSetEntity> adSetRepository,
            IBaseRepository<AdsAccount> accountRepository)
        {
            _adSetRepository = adSetRepository;
            _accountRepository = accountRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResultDto<AdSetListItemDto>> GetListAsync(GetAdSetListInput input)
        {
            var adSetQuery = await _adSetRepository.GetQueryableAsync();
            var accountQuery = await _accountRepository.GetQueryableAsync();

            // JOIN: adset → account（获取平台信息）
            var query = from ads in adSetQuery
                        join acc in accountQuery on ads.AccountId equals acc.Id
                        select new { AdSet = ads, Account = acc };

            // AccountIds 过滤
            if (!input.AccountIds.IsNullOrEmpty() && input.AccountIds!.Any())
                query = query.Where(x => input.AccountIds!.Contains(x.AdSet.AccountId));

            // CampaignIds 过滤
            if (!input.CampaignIds.IsNullOrEmpty() && input.CampaignIds!.Any())
                query = query.Where(x => input.CampaignIds!.Contains(x.AdSet.CampaignId));

            if (input.AccountId.HasValue)
                query = query.Where(x => x.AdSet.AccountId == input.AccountId.Value);

            // Platform 过滤
            if (input.Platform.HasValue)
                query = query.Where(x => x.Account.Platform == input.Platform.Value);

            // AdSetName 模糊搜索
            if (!string.IsNullOrWhiteSpace(input.AdSetName))
                query = query.Where(x => x.AdSet.AdSetName.Contains(input.AdSetName));

            // AdSetNo 精确筛选
            if (!input.AdSetNo.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.AdSet.AdSetNo == input.AdSetNo);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.AdSet.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Select(x => new AdSetListItemDto
                {
                    AdSetId = x.AdSet.Id,
                    AdSetNo = x.AdSet.AdSetNo,
                    AdSetName = x.AdSet.AdSetName,
                    MediaState = x.AdSet.MediaState,
                    BudgetType = x.AdSet.BudgetType,
                    Budget = x.AdSet.Budget,
                    CampaignNo = x.AdSet.CampaignNo,
                    AccountNo = x.AdSet.AccountNo,
                    MediaCreateTime = x.AdSet.MediaCreateTime,
                    CreationTime = x.AdSet.CreationTime,
                })
                .ToListAsync();

            return new PagedResultDto<AdSetListItemDto>(totalCount, items);
        }
    }
}
