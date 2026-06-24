using Ads.Automation.Application.Contracts.Entity.Departments;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Ads.Automation.Api.Controllers.System;

[Route("api/system/departments")]
[ApiController]
public class DepartmentsController : ApiControllerBase
{
    private readonly ISysDepartmentAppService _departmentAppService;

    public DepartmentsController(ISysDepartmentAppService departmentAppService)
    {
        _departmentAppService = departmentAppService;
    }

    /// <summary>
    /// 获取部门信息
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        return Success(await _departmentAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取部门列表信息
    /// </summary>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetSysDepartmentListInput input)
    {
        return Success(await _departmentAppService.GetListAsync(input));
    }

    /// <summary>
    /// 创建部门信息
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateSysDepartmentDto input)
    {
        return Success(await _departmentAppService.CreateAsync(input));
    }

    /// <summary>
    /// 更新部门信息
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CreateUpdateSysDepartmentDto input)
    {
        return Success(await _departmentAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除部门信息
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _departmentAppService.DeleteAsync(id);
        return Success<object?>(null);
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <returns></returns>
    [HttpPost("tree")]
    public async Task<IActionResult> GetTreeAsync([FromBody] GetSysDepartmentListInput input)
    {
        return Success(await _departmentAppService.GetTreeAsync(input));
    }
}
