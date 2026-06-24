
namespace Ads.Automation.Application.Entity.Pages
{
    /// <summary>
    /// page主页信息AppService实现
    /// </summary>
    public class PagesAppService : ApplicationService, IPagesAppService
    {
        private readonly IBaseRepository<AdsPage> _pagesRepository;
        private readonly IBaseRepository<AdsChannelAdAccount> _channelAdAccountRepository;
        private readonly IBaseRepository<AdsChannelPage> _channelPageRepository; 

        public PagesAppService(
            IBaseRepository<AdsPage> pagesRepository,
            IBaseRepository<AdsChannelAdAccount> channelAdAccountRepository, IBaseRepository<AdsChannelPage> channelPageRepository)
        {
            _pagesRepository = pagesRepository;
            _channelAdAccountRepository = channelAdAccountRepository;
            _channelPageRepository = channelPageRepository;
        }

        public async Task<PagedResultDto<PagesDto>> GetListAsync(GetPageListInput input)
        {
            var query = await _pagesRepository.GetQueryableAsync();

            // 按个号（ChannelId）筛选：通过 AdsChannelAdAccount 表关联
            if (input.ChannelId.HasValue)
            {
                    var pageIds = await _channelPageRepository.GetQueryableAsync();
                    var channelPageIds = pageIds
                        .Where(r => r.ChannelId == input.ChannelId.Value)
                        .Select(r => r.PageId)
                        .Distinct();
    
                    query = query.Where(p => channelPageIds.Contains(p.Id));
            }

            // 按主页名称/编号过滤
            if (!string.IsNullOrWhiteSpace(input.Filter))
            {
                query = query.Where(p =>
                    p.PageName.Contains(input.Filter) ||
                    p.PageNo.Contains(input.Filter));
            }

            var totalCount = query.Count();
            if (totalCount == 0) 
                return new PagedResultDto<PagesDto>(0, new List<PagesDto>());

            var items = query
                .OrderBy(p => p.Id)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToList();

            var dtos = ObjectMapper.Map<List<AdsPage>, List<PagesDto>>(items);

            return new PagedResultDto<PagesDto>(totalCount, dtos);
        }
    }
}
