using Ads.Automation.Application.Contracts.Entity.Menus;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Ads.Automation.Api.Controllers.System;

/// <summary>
/// 菜单信息控制器
/// </summary>
[Route("api/system/menus")]
[ApiController]
public class SysMenuController : ApiControllerBase
{
    private readonly ISysMenusAppService _menusAppService;

    public SysMenuController(ISysMenusAppService menusAppService)
    {
        _menusAppService = menusAppService;
    }

    /// <summary>
    /// 获取菜单信息
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        return Success(await _menusAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取菜单列表
    /// </summary>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetSysMenusListInput input)
    {
        return Success(await _menusAppService.GetListAsync(input));
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateSysMenusDto input)
    {
        return Success(await _menusAppService.CreateAsync(input));
    }

    /// <summary>
    /// 修改菜单
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CreateUpdateSysMenusDto input)
    {
        return Success(await _menusAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _menusAppService.DeleteAsync(id);
        return Success<object?>(null);
    }

    /// <summary>
    /// 获取菜单树形结构
    /// </summary>
    [HttpPost("tree")]
    public async Task<IActionResult> GetTreeAsync([FromBody] GetSysMenusListInput input)
    {
        return Success(await _menusAppService.GetTreeAsync(input));
    }
}
