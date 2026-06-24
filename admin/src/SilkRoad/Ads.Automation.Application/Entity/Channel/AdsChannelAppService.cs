
namespace Ads.Automation.Application.Entity.Channel;

/// <summary>
/// 广告渠道应用服务实现
/// </summary>
public class AdsChannelAppService : ApplicationService, IAdsChannelAppService
{
    private readonly IBaseRepository<AdsChannel> _channelRepository;
    private readonly UserInfoContext _user;

    public AdsChannelAppService(IBaseRepository<AdsChannel> channelRepository, UserInfoContext user)
    {
        _channelRepository = channelRepository;
        _user = user;
    }

    public async Task<AdsChannelDto> GetAsync(long id)
    {
        var channel = await _channelRepository.GetAsync(c => c.Id == id);
        return ObjectMapper.Map<AdsChannel, AdsChannelDto>(channel);
    }

    public async Task<PagedResultDto<AdsChannelListDto>> GetListAsync(GetAdsChannelListInput input)
    {
        var query = await _channelRepository.GetQueryableAsync();

        // 只查未删除的 和 当前用户相关的渠道
        query = query.Where(c => !c.IsDeleted);
            //.Where(e => e.CreatorId == _user.UserId);

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(c => c.ChannelName.Contains(input.FilterText));
        }

        if (input.Platform.HasValue)
        {
            query = query.Where(c => c.Platform == input.Platform.Value);
        }

        if (input.ChannelState.HasValue)
        {
            query = query.Where(c => c.ChannelState == input.ChannelState.Value);
        }

        var totalCount = query.Count();

        query = query
            .OrderBy(c => c.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = ObjectMapper.Map<List<AdsChannel>, List<AdsChannelListDto>>(query.ToList());

        return new PagedResultDto<AdsChannelListDto>(totalCount, items);
    }
    

    public async Task DeleteAsync(long id)
    {
        await _channelRepository.DeleteAsync(c => c.Id == id);
    }
}
