using Ads.Automation.Application.Contracts.Entity.Ads;
using Ads.Automation.Domain.Account;
using Ads.Automation.Domain.Ads;

namespace Ads.Automation.Application.Entity.Ads
{
    public class AdAppService : ApplicationService, IAdAppService
    {
        private readonly IBaseRepository<AdEntity> _adRepository;
        private readonly IBaseRepository<AdsAccount> _accountRepository;

        public AdAppService(
            IBaseRepository<AdEntity> adRepository,
            IBaseRepository<AdsAccount> accountRepository)
        {
            _adRepository = adRepository;
            _accountRepository = accountRepository;
        }

        /// <inheritdoc />
        public async Task<PagedResultDto<AdListItemDto>> GetListAsync(GetAdListInput input)
        {
            var adQuery = await _adRepository.GetQueryableAsync();
            var accountQuery = await _accountRepository.GetQueryableAsync();

            // JOIN: ad → account（获取平台信息）
            var query = from ad in adQuery
                        join acc in accountQuery on ad.AccountId equals acc.Id
                        select new { Ad = ad, Account = acc };

            // AccountIds 过滤
            if (!input.AccountIds.IsNullOrEmpty() && input.AccountIds!.Any())
                query = query.Where(x => input.AccountIds!.Contains(x.Ad.AccountId));

            // CampaignIds 过滤
            if (!input.CampaignIds.IsNullOrEmpty() && input.CampaignIds!.Any())
                query = query.Where(x => input.CampaignIds!.Contains(x.Ad.CampaignId));

            // AdSetIds 过滤
            if (!input.AdSetIds.IsNullOrEmpty() && input.AdSetIds!.Any())
                query = query.Where(x => input.AdSetIds!.Contains(x.Ad.AdSetId));

            // Platform 过滤
            if (input.Platform.HasValue)
                query = query.Where(x => x.Account.Platform == input.Platform.Value);

            // AdName 模糊搜索
            if (!string.IsNullOrWhiteSpace(input.AdName))
                query = query.Where(x => x.Ad.AdName.Contains(input.AdName));

            // AdNo 精确筛选
            if (!input.AdNo.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Ad.AdNo == input.AdNo);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Ad.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Select(x => new AdListItemDto
                {
                    AdId = x.Ad.Id,
                    AdNo = x.Ad.AdNo,
                    AdName = x.Ad.AdName,
                    MediaState = x.Ad.MediaState,
                    CreativeNo = x.Ad.CreativeNo,
                    PageNo = x.Ad.PageNo,
                    CampaignNo = x.Ad.CampaignNo,
                    AdSetNo = x.Ad.AdSetNo,
                    AccountNo = x.Ad.AccountNo,
                    MediaCreateTime = x.Ad.MediaCreateTime,
                    CreationTime = x.Ad.CreationTime,
                })
                .ToListAsync();

            return new PagedResultDto<AdListItemDto>(totalCount, items);
        }
    }
}
