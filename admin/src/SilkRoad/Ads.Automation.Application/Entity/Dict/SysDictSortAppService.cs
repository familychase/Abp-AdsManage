using BusinessException = Volo.Abp.BusinessException;

namespace Ads.Automation.Application.Entity.Dict;

/// <summary>
/// 系统字典类 AppService 实现
/// </summary>
public class SysDictSortAppService : ApplicationService, ISysDictSortAppService
{
    private readonly IBaseRepository<SysDictSort> _dictSortRepository;
    private readonly IBaseRepository<SysDictItem> _dictItemRepository;

    public SysDictSortAppService(
        IBaseRepository<SysDictSort> dictSortRepository,
        IBaseRepository<SysDictItem> dictItemRepository)
    {
        _dictSortRepository = dictSortRepository;
        _dictItemRepository = dictItemRepository;
    }

    public async Task<SysDictSortDto> GetAsync(long id)
    {
        var entity = await _dictSortRepository.GetAsync(d => d.Id == id);
        return ObjectMapper.Map<SysDictSort, SysDictSortDto>(entity);
    }

    public async Task<PagedResultDto<SysDictSortDto>> GetListAsync(GetSysDictSortListInput input)
    {
        var query = await _dictSortRepository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Keyword))
        {
            query = query.Where(d =>
                d.DictSortCode.Contains(input.Keyword) ||
                d.DictSortName.Contains(input.Keyword));
        }

        if (input.DictSortType.HasValue)
            query = query.Where(d => d.DictSortType == input.DictSortType.Value);

        var totalCount = await query.CountAsync();
        var list = await query
            .OrderByDescending(d => d.LastModificationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToListAsync();

        return new PagedResultDto<SysDictSortDto>(
            totalCount,
            ObjectMapper.Map<List<SysDictSort>, List<SysDictSortDto>>(list));
    }

    public async Task<SysDictSortDto> CreateAsync(CreateUpdateSysDictSortDto input)
    {
        var existing = await _dictSortRepository.GetQueryableAsync();
        var codeExists = await existing.AnyAsync(d => d.DictSortCode == input.DictSortCode);
        if (codeExists)
            throw new BusinessException("Dict:CodeExists");

        var entity = SysDictSort.Create(
            input.Platform, input.DictSortType,
            input.DictSortCode, input.DictSortName, input.Remarks);
        await _dictSortRepository.InsertAsync(entity);

        return ObjectMapper.Map<SysDictSort, SysDictSortDto>(entity);
    }

    public async Task<SysDictSortDto> UpdateAsync(long id, CreateUpdateSysDictSortDto input)
    {
        var entity = await _dictSortRepository.GetAsync(d => d.Id == id);

        // 检查编码唯一性
        var existing = await _dictSortRepository.GetQueryableAsync();
        var codeExists = await existing.AnyAsync(
            d => d.DictSortCode == input.DictSortCode && d.Id != id);
        if (codeExists)
            throw new BusinessException("Dict:CodeExists");

        entity.SetPlatform(input.Platform);
        entity.SetDictSortType(input.DictSortType);
        entity.SetDictSortCode(input.DictSortCode);
        entity.SetDictSortName(input.DictSortName);
        entity.SetRemarks(input.Remarks);
        await _dictSortRepository.UpdateAsync(entity);

        return ObjectMapper.Map<SysDictSort, SysDictSortDto>(entity);
    }

    public async Task DeleteAsync(long id)
    {
        // 检查是否存在字典项
        var itemQuery = await _dictItemRepository.GetQueryableAsync();
        var hasItems = await itemQuery.AnyAsync(d => d.DictSortId == id);
        if (hasItems)
            throw new BusinessException("Dict:SortHasItems");

        await _dictSortRepository.DeleteAsync(d => d.Id == id);
    }

    public async Task AddSortWithItemsAsync(CreateUpdateDictSortWithItemsDto input)
    {
        // 检查 DictItems 内部是否存在重复 DictItemCode（大小写不敏感，对齐 SQL Server 默认排序规则）
        var duplicateCodes = input.DictItems
            .GroupBy(i => i.DictItemCode, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateCodes.Any())
        {
            throw new BusinessException(
                $"Dict item code '{string.Join("', '", duplicateCodes)}' duplicate, please check input");
        }

        input.DictItems.ForEach(e =>
        {
            if (e.DictSortId == e.ParentId)
            {
                e.ParentId = 0;
            }
        });

        // 查找已有字典类
        var sortQuery = await _dictSortRepository.GetQueryableAsync();
        var existingSort = await sortQuery.FirstOrDefaultAsync(s => s.DictSortCode == input.DictSortCode);

        if (existingSort == null)
        {
            // 不存在 → 新建 sort + items
            var sort = SysDictSort.Create(
                input.Platform, input.DictSortType,
                input.DictSortCode, input.DictSortName, input.Remarks);
            await _dictSortRepository.InsertAsync(sort);

            foreach (var itemDto in input.DictItems)
            {
                var item = SysDictItem.Create(
                    sort.Id, itemDto.DictItemCode, itemDto.DictItemName,
                    itemDto.DictItemNameEN, itemDto.DictItemValue, itemDto.Remarks,
                    itemDto.Ordinal, itemDto.ItemType, itemDto.IsProduction, itemDto.ParentId);
                await _dictItemRepository.InsertAsync(item);
            }

            return;
        }

        // 存在 → 更新 sort 属性 + 全量替换 items
        existingSort.SetPlatform(input.Platform);
        existingSort.SetDictSortType(input.DictSortType);
        existingSort.SetDictSortName(input.DictSortName);
        existingSort.SetRemarks(input.Remarks);
        await _dictSortRepository.UpdateAsync(existingSort);

        // var parentId = input.DictItems.FirstOrDefault()?.ParentId ?? 0;

        // Upsert：已存在的更新，不存在的插入，避免 DELETE+INSERT 顺序问题
        var itemQuery = await _dictItemRepository.GetQueryableAsync();
        foreach (var itemDto in input.DictItems)
        {
            var existingItem = await itemQuery.FirstOrDefaultAsync(i =>
                i.DictSortId == existingSort.Id && i.DictItemCode == itemDto.DictItemCode);

            if (existingItem != null)
            {
                existingItem.SetDictItemName(itemDto.DictItemName);
                existingItem.SetDictItemNameEN(itemDto.DictItemNameEN);
                existingItem.SetDictItemValue(itemDto.DictItemValue);
                existingItem.SetRemarks(itemDto.Remarks);
                existingItem.SetOrdinal(itemDto.Ordinal);
                existingItem.SetItemType(itemDto.ItemType);
                existingItem.SetIsProduction(itemDto.IsProduction);
                existingItem.SetParentId(itemDto.ParentId);
                await _dictItemRepository.UpdateAsync(existingItem);
            }
            else
            {
                var item = SysDictItem.Create(
                    existingSort.Id, itemDto.DictItemCode, itemDto.DictItemName,
                    itemDto.DictItemNameEN, itemDto.DictItemValue, itemDto.Remarks,
                    itemDto.Ordinal, itemDto.ItemType, itemDto.IsProduction, itemDto.ParentId);
                await _dictItemRepository.InsertAsync(item);
            }
        }
    }

    public async Task<DateTime?> GetLastUpdateTimeAsync()
    {
        var query = await _dictSortRepository.GetQueryableAsync();
        var sortTime = await query.MaxAsync(d => (DateTime?)d.LastModificationTime);

        var itemQuery = await _dictItemRepository.GetQueryableAsync();
        var itemTime = await itemQuery.MaxAsync(d => (DateTime?)d.LastModificationTime);

        if (sortTime == null && itemTime == null) return null;
        if (sortTime == null) return itemTime;
        if (itemTime == null) return sortTime;
        return sortTime > itemTime ? sortTime : itemTime;
    }
}
