using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Ads.Automation.Application.Contracts.Entity.Dict;

namespace Ads.Automation.Api.Controllers.System;

/// <summary>
/// 系统字典控制器
/// </summary>
[Route("api/system/dict")]
[ApiController]
public class SysDictController : ApiControllerBase
{
    private readonly ISysDictSortAppService _dictSortAppService;
    private readonly ISysDictItemAppService _dictItemAppService;

    public SysDictController(
        ISysDictSortAppService dictSortAppService,
        ISysDictItemAppService dictItemAppService)
    {
        _dictSortAppService = dictSortAppService;
        _dictItemAppService = dictItemAppService;
    }

    // ===== 字典类 =====

    /// <summary>
    /// 获取字典类
    /// </summary>
    [HttpGet("sort/{id}")]
    public async Task<IActionResult> GetSortAsync(long id)
    {
        return Success(await _dictSortAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取字典类列表
    /// </summary>
    [HttpPost("sort/list")]
    public async Task<IActionResult> GetSortListAsync([FromBody] GetSysDictSortListInput input)
    {
        return Success(await _dictSortAppService.GetListAsync(input));
    }

    /// <summary>
    /// 创建字典类
    /// </summary>
    [HttpPost("sort")]
    public async Task<IActionResult> CreateSortAsync([FromBody] CreateUpdateSysDictSortDto input)
    {
        return Success(await _dictSortAppService.CreateAsync(input));
    }

    /// <summary>
    /// 修改字典类
    /// </summary>
    [HttpPut("sort/{id}")]
    public async Task<IActionResult> UpdateSortAsync(long id, [FromBody] CreateUpdateSysDictSortDto input)
    {
        return Success(await _dictSortAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除字典类
    /// </summary>
    [HttpDelete("sort/{id}")]
    public async Task<IActionResult> DeleteSortAsync(long id)
    {
        await _dictSortAppService.DeleteAsync(id);
        return Success<object?>(null);
    }

    // ===== 字典项 =====

    /// <summary>
    /// 获取字典项
    /// </summary>
    [HttpGet("item/{id}")]
    public async Task<IActionResult> GetItemAsync(long id)
    {
        return Success(await _dictItemAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取字典项列表
    /// </summary>
    [HttpPost("item/list")]
    public async Task<IActionResult> GetItemListAsync([FromBody] GetSysDictItemListInput input)
    {
        return Success(await _dictItemAppService.GetListAsync(input));
    }

    /// <summary>
    /// 获取字典项树形结构
    /// </summary>
    [HttpGet("item/tree/{dictSortId}")]
    public async Task<IActionResult> GetItemTreeAsync(long dictSortId)
    {
        return Success(await _dictItemAppService.GetTreeAsync(dictSortId));
    }

    /// <summary>
    /// 创建字典项
    /// </summary>
    [HttpPost("item")]
    public async Task<IActionResult> CreateItemAsync([FromBody] CreateUpdateSysDictItemDto input)
    {
        return Success(await _dictItemAppService.CreateAsync(input));
    }

    /// <summary>
    /// 修改字典项
    /// </summary>
    [HttpPut("item/{id}")]
    public async Task<IActionResult> UpdateItemAsync(long id, [FromBody] CreateUpdateSysDictItemDto input)
    {
        return Success(await _dictItemAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除字典项
    /// </summary>
    [HttpDelete("item/{id}")]
    public async Task<IActionResult> DeleteItemAsync(long id)
    {
        await _dictItemAppService.DeleteAsync(id);
        return Success<object?>(null);
    }

    /// <summary>
    /// 批量获取多个字典类编码对应的字典项（web_list 接口）
    /// 不传或传 null 时查询全部字典
    /// </summary>
    [HttpPost("item/web_list")]
    public async Task<IActionResult> GetItemWebListAsync([FromBody] List<string>? sortCodes)
    {
        return Success(await _dictItemAppService.GetItemsBySortCodesAsync(sortCodes));
    }

    /// <summary>
    /// 获取字典项列表（非分页，按 sort_id 或 sort_code 查询）
    /// </summary>
    [HttpGet("item_list")]
    public async Task<IActionResult> GetItemListBySortAsync([FromQuery] long? sort_id, [FromQuery] string? sort_code)
    {
        return Success(await _dictItemAppService.GetItemsBySortAsync(sort_id, sort_code));
    }

    /// <summary>
    /// 批量创建字典类 + 字典项
    /// </summary>
    [HttpPost("sort_item/add")]
    public async Task<IActionResult> AddSortWithItemsAsync([FromBody] CreateUpdateDictSortWithItemsDto input)
    {
        await _dictSortAppService.AddSortWithItemsAsync(input);
        return Success<object?>(null);
    }

    /// <summary>
    /// 获取字典最后更新时间
    /// </summary>
    [HttpGet("last/update_time")]
    public async Task<IActionResult> GetLastUpdateTimeAsync()
    {
        return Success(await _dictSortAppService.GetLastUpdateTimeAsync());
    }
}
