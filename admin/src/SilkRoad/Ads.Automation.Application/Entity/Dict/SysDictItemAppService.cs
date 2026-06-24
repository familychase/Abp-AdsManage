using BusinessException = Volo.Abp.BusinessException;
using Volo.Abp.Validation;

namespace Ads.Automation.Application.Entity.Dict;

/// <summary>
/// 系统字典项 AppService 实现
/// </summary>
public class SysDictItemAppService : ApplicationService, ISysDictItemAppService
{
    private readonly IBaseRepository<SysDictItem> _dictItemRepository;
    private readonly IBaseRepository<SysDictSort> _dictSortRepository;

    public SysDictItemAppService(
        IBaseRepository<SysDictItem> dictItemRepository,
        IBaseRepository<SysDictSort> dictSortRepository)
    {
        _dictItemRepository = dictItemRepository;
        _dictSortRepository = dictSortRepository;
    }

    public async Task<SysDictItemDetailDto> GetAsync(long id)
    {
        var entity = await _dictItemRepository.GetAsync(d => d.Id == id);
        var dto = ObjectMapper.Map<SysDictItem, SysDictItemDetailDto>(entity);

        if (dto.DictSortId > 0)
        {
            var sort = await _dictSortRepository.FindAsync(s => s.Id == dto.DictSortId);
            if (sort != null) dto.DictSortName = sort.DictSortName;
        }

        return dto;
    }

    public async Task<PagedResultDto<SysDictItemDetailDto>> GetListAsync(GetSysDictItemListInput input)
    {
        var query = await _dictItemRepository.GetQueryableAsync();

        if (input.DictSortId.HasValue)
            query = query.Where(d => d.DictSortId == input.DictSortId.Value);

        if (input.ParentId.HasValue)
            query = query.Where(d => d.ParentId == input.ParentId.Value);

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(d =>
                d.DictItemCode.Contains(input.Keyword) ||
                d.DictItemName.Contains(input.Keyword) ||
                d.DictItemNameEN.Contains(input.Keyword));
        }

        var totalCount = await query.CountAsync();
        var list = await query
            .OrderBy(d => d.Ordinal)
            .ThenBy(d => d.Id)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        var dtos = ObjectMapper.Map<List<SysDictItem>, List<SysDictItemDetailDto>>(list);

        // 填充 DictSortName
        if (dtos.Count > 0)
        {
            var sortIds = dtos.Select(d => d.DictSortId).Distinct().ToList();
            var sorts = (await _dictSortRepository.GetListAsync(s => sortIds.Contains(s.Id)))
                .ToDictionary(k => k.Id, v => v.DictSortName);
            foreach (var item in dtos)
                if (sorts.TryGetValue(item.DictSortId, out var name))
                    item.DictSortName = name;
        }

        return new PagedResultDto<SysDictItemDetailDto>(totalCount, dtos);
    }

    public async Task<List<SysDictItemTreeNodeDto>> GetTreeAsync(long dictSortId)
    {
        var query = await _dictItemRepository.GetQueryableAsync();
        var allItems = await query
            .Where(d => d.DictSortId == dictSortId)
            .OrderBy(d => d.ParentId)
            .ThenBy(d => d.Ordinal)
            .ThenBy(d => d.Id)
            .ToListAsync();

        var dtos = ObjectMapper.Map<List<SysDictItem>, List<SysDictItemTreeNodeDto>>(allItems);

        // 构建树形结构
        var dict = dtos.ToDictionary(d => d.Id);
        var tree = new List<SysDictItemTreeNodeDto>();

        foreach (var dto in dtos)
        {
            if (dto.ParentId == 0 || !dict.ContainsKey(dto.ParentId))
                tree.Add(dto);
            else
                dict[dto.ParentId].Children.Add(dto);
        }

        return tree;
    }

    public async Task<SysDictItemDetailDto> CreateAsync(CreateUpdateSysDictItemDto input)
    {
        // 编码唯一性校验
        var existing = await _dictItemRepository.GetQueryableAsync();
        var codeExists = await existing.AnyAsync(
            d => d.DictSortId == input.DictSortId && d.DictItemCode == input.DictItemCode);
        if (codeExists)
            throw new BusinessException("Dict:ItemCodeExists");

        var entity = SysDictItem.Create(
            input.DictSortId, input.DictItemCode, input.DictItemName,
            input.DictItemNameEN, input.DictItemValue, input.Remarks,
            input.Ordinal, input.ItemType, input.IsProduction, input.ParentId);

        await _dictItemRepository.InsertAsync(entity);

        var dto = ObjectMapper.Map<SysDictItem, SysDictItemDetailDto>(entity);
        if (dto.DictSortId > 0)
        {
            var sort = await _dictSortRepository.FindAsync(s => s.Id == dto.DictSortId);
            if (sort != null) dto.DictSortName = sort.DictSortName;
        }

        return dto;
    }

    public async Task<SysDictItemDetailDto> UpdateAsync(long id, CreateUpdateSysDictItemDto input)
    {
        var query = await _dictItemRepository.GetQueryableAsync();
        var entity = await query.FirstOrDefaultAsync(d => d.Id == id);
        if (entity == null)
            throw new BusinessException("Dict:ItemNotFound");

        // 编码唯一性校验
        var existing = await _dictItemRepository.GetQueryableAsync();
        var codeExists = await existing.AnyAsync(
            d => d.DictSortId == input.DictSortId
                && d.DictItemCode == input.DictItemCode
                && d.Id != id);
        if (codeExists)
            throw new BusinessException("Dict:ItemCodeExists");

        entity.SetDictSortId(input.DictSortId);
        entity.SetParentId(input.ParentId);
        entity.SetDictItemCode(input.DictItemCode);
        entity.SetDictItemName(input.DictItemName);
        entity.SetDictItemNameEN(input.DictItemNameEN);
        entity.SetDictItemValue(input.DictItemValue);
        entity.SetRemarks(input.Remarks);
        entity.SetOrdinal(input.Ordinal);
        entity.SetItemType(input.ItemType);
        entity.SetIsProduction(input.IsProduction);

        await _dictItemRepository.UpdateAsync(entity);

        var dto = ObjectMapper.Map<SysDictItem, SysDictItemDetailDto>(entity);
        if (dto.DictSortId > 0)
        {
            var sort = await _dictSortRepository.FindAsync(s => s.Id == dto.DictSortId);
            if (sort != null) dto.DictSortName = sort.DictSortName;
        }

        return dto;
    }

    public async Task DeleteAsync(long id)
    {
        // 检查是否有子级
        var itemQuery = await _dictItemRepository.GetQueryableAsync();
        var hasChildren = await itemQuery.AnyAsync(d => d.ParentId == id);
        if (hasChildren)
            throw new BusinessException("Dict:ItemHasChildren");

        await _dictItemRepository.DeleteAsync(d => d.Id == id);
    }

    [DisableValidation]
    public async Task<Dictionary<string, List<SysDictItemDto>>> GetItemsBySortCodesAsync(List<string>? sortCodes)
    {
        var codes = sortCodes?.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();

        // 1. 查询字典类：传了编码则按编码过滤，否则查全部
        var sortQuery = await _dictSortRepository.GetQueryableAsync();
        var allSorts = await sortQuery
            .OrderBy(s => s.DictSortCode)
            .ToListAsync();

        var sorts = codes is { Count: > 0 }
            ? allSorts.Where(s => codes.Contains(s.DictSortCode)).ToList()
            : allSorts;

        if (sorts.Count == 0)
            return new Dictionary<string, List<SysDictItemDto>>();

        var sortIds = sorts.Select(s => s.Id).ToList();
        var sortCodeMap = sorts.ToDictionary(k => k.Id, v => v.DictSortCode);

        // 2. 查询字典项
        var itemQuery = await _dictItemRepository.GetQueryableAsync();
        var items = await itemQuery
            .Where(i => sortIds.Contains(i.DictSortId) && i.IsProduction)
            .OrderBy(i => i.Ordinal)
            .ThenBy(i => i.Id)
            .ToListAsync();

        // 3. 按 DictSortCode 分组
        var result = new Dictionary<string, List<SysDictItemDto>>();
        foreach (var sort in sorts)
        {
            result[sort.DictSortCode] = new List<SysDictItemDto>();
        }

        var isEnglish = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            if (sortCodeMap.TryGetValue(item.DictSortId, out var code))
            {
                var dto = ObjectMapper.Map<SysDictItem, SysDictItemDto>(item);
                if (isEnglish)
                {
                    dto.DictItemName = item.DictItemNameEN;
                }
                result[code].Add(dto);
            }
        }

        return result;
    }

    public async Task<List<SysDictItemDto>> GetItemsBySortAsync(long? sortId, string? sortCode)
    {
        if (!sortId.HasValue && string.IsNullOrWhiteSpace(sortCode))
            return new List<SysDictItemDto>();

        // 通过 DictSortCode 查 ID
        long? resolvedSortId = sortId;
        if (!resolvedSortId.HasValue && !string.IsNullOrWhiteSpace(sortCode))
        {
            var sortQuery = await _dictSortRepository.GetQueryableAsync();
            var sort = await sortQuery.FirstOrDefaultAsync(s => s.DictSortCode == sortCode);
            if (sort == null) return new List<SysDictItemDto>();
            resolvedSortId = sort.Id;
        }

        if (!resolvedSortId.HasValue)
            return new List<SysDictItemDto>();

        var itemQuery = await _dictItemRepository.GetQueryableAsync();
        var items = await itemQuery
            .Where(i => i.DictSortId == resolvedSortId.Value)
            .OrderBy(i => i.Ordinal)
            .ThenBy(i => i.Id)
            .ToListAsync();

        return ObjectMapper.Map<List<SysDictItem>, List<SysDictItemDto>>(items);
    }
}
