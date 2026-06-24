using Ads.Automation.Domain.Pixel;

namespace Ads.Automation.Application.Entity.Pixel;

/// <summary>
/// 像素应用服务实现
/// </summary>
public class PixelAppService : ApplicationService, IPixelAppService
{
    private readonly IBaseRepository<AdsPixel> _pixelRepository;
    private readonly IBaseRepository<AdsAccountPixel> _accountPixelRepository;

    public PixelAppService(
        IBaseRepository<AdsPixel> pixelRepository,
        IBaseRepository<AdsAccountPixel> accountPixelRepository)
    {
        _pixelRepository = pixelRepository;
        _accountPixelRepository = accountPixelRepository;
    }

    public async Task<PagedResultDto<PixelDto>> GetListAsync(GetPixelListInput input)
    {
        var query = (await _pixelRepository.GetQueryableAsync()).AsNoTracking();

        // 按关联账户筛选：通过 ads_account_pixel 表过滤
        if (!string.IsNullOrWhiteSpace(input.AccountNo))
        {
            var filterRelationQuery = await _accountPixelRepository.GetQueryableAsync();
            var filterPixelIds = filterRelationQuery
                .Where(r => r.AccountNo == input.AccountNo)
                .Select(r => r.PixelId)
                .Distinct();
            query = query.Where(p => filterPixelIds.Contains(p.Id));
        }

        // 关键字模糊匹配 PixelNo / PixelName
        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            query = query.Where(p =>
                p.PixelName.Contains(input.FilterText) ||
                p.PixelNo.Contains(input.FilterText));
        }

        var totalCount = await query.CountAsync();
        if (totalCount == 0)
            return new PagedResultDto<PixelDto>(0, new List<PixelDto>());

        var pixels = await query
            .OrderByDescending(p => p.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        // 批量查询关联关系，避免 N+1
        var pixelIds = pixels.Select(p => p.Id).ToList();
        var relationQuery = await _accountPixelRepository.GetQueryableAsync();
        var allRelations = await relationQuery
            .Where(r => pixelIds.Contains(r.PixelId))
            .ToListAsync();

        var accountMap = allRelations
            .GroupBy(r => r.PixelId)
            .ToDictionary(g => g.Key, g => g.Select(r => r.AccountNo).ToList());

        var dtos = pixels.Select(p =>
        {
            var dto = ObjectMapper.Map<AdsPixel, PixelDto>(p);
            if (accountMap.TryGetValue(p.Id, out var accounts))
            {
                dto.AccountCount = accounts.Count;
                dto.AssociatedAccounts = accounts;
            }
            return dto;
        }).ToList();

        return new PagedResultDto<PixelDto>(totalCount, dtos);
    }
}
