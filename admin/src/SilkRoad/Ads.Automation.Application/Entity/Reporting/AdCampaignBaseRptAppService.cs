using Ads.Automation.Domain.Reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.Application.Entity.Reporting
{
    public class AdCampaignBaseRptAppService : ApplicationService, IAdCampaignBaseRptAppService
    {
        private readonly IBaseRepository<AdCampaignBaseRpt> _campRptRepository;
        private readonly IBaseRepository<AdsAccount> _accountRepository;

        public AdCampaignBaseRptAppService(IBaseRepository<AdCampaignBaseRpt> campRptRepository, IBaseRepository<AdsAccount> accountRepository)
        {
            _campRptRepository = campRptRepository;
            _accountRepository = accountRepository;
        }

        /// <inheritdoc/>
        public async Task<PagedResultDto<CampaignBaseRptDto>> GetListAsync(GetCampaignBaseRptListInput input)
        {
            var query = (await _campRptRepository.GetQueryableAsync()).AsNoTracking();
            if (!input.AccountNo.IsNullOrEmpty())
            {
                var accIds = await (await _accountRepository.GetQueryableAsync()).Where(e => e.AccountNo == input.AccountNo).Select(s => s.Id).ToListAsync();
                query = query.Where(e => accIds.Contains(e.AccountId));
            }

            if (!input.CampaignNo.IsNullOrEmpty())
            {
                query = query.Where(e => e.CampaignNo == input.CampaignNo);
            }

            if (!input.CampaignName.IsNullOrEmpty())
            {
                query = query.Where(e => EF.Functions.Like(e.CampaignName, $"%{input.CampaignName}%"));
            }

            if (input.DateRange != null)
            {
                query = query.Where(e => e.ReportDate >= input.DateRange.Start && e.ReportDate <= input.DateRange.Stop);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.Id)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .Select(r => new CampaignBaseRptDto
                {
                    Id = r.Id,
                    AccountId = r.AccountId,
                    CampaignNo = r.CampaignNo,
                    CampaignName = r.CampaignName,
                    ReportDate = r.ReportDate,
                    Platform = r.Platform,
                    Spend = r.Spend,
                    Clicks = r.Clicks,
                    Converts = r.Converts,
                    Impressions = r.Impressions,
                    CPC = r.CPC,
                })
                .ToListAsync();

            var accountIds = items.Select(s => s.AccountId).Distinct().ToList();
            var accounts = await (await _accountRepository.GetQueryableAsync()).Where(e => accountIds.Contains(e.Id)).ToDictionaryAsync(k => k.Id!, v => new { v.AccountName, v.AccountNo });
            foreach (var item in items)
            {
                var account = accounts.FirstOrDefault(e => e.Key == item.AccountId);
                item.AccountName = account.Value!.AccountName!;
                item.AccountNo = account.Value!.AccountNo!;
            }

            return new PagedResultDto<CampaignBaseRptDto>(totalCount, items);
        }
    }
}
