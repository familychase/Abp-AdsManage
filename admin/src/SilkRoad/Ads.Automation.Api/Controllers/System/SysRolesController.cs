using Ads.Automation.Application.Contracts.Entity.SysRoles;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Ads.Automation.Api.Controllers.System;

/// <summary>
/// 角色信息控制器
/// </summary>
[Route("api/system/roles")]
[ApiController]
public class SysRolesController : ApiControllerBase
{
    private readonly ISysRolesAppService _rolesAppService;

    public SysRolesController(ISysRolesAppService rolesAppService)
    {
        _rolesAppService = rolesAppService;
    }

    /// <summary>
    /// 获取角色信息
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        return Success(await _rolesAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetSysRolesListInput input)
    {
        return Success(await _rolesAppService.GetListAsync(input));
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateSysRolesDto input)
    {
        return Success(await _rolesAppService.CreateAsync(input));
    }

    /// <summary>
    /// 修改角色
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CreateUpdateSysRolesDto input)
    {
        return Success(await _rolesAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _rolesAppService.DeleteAsync(id);
        return Success<object?>(null);
    }
}
