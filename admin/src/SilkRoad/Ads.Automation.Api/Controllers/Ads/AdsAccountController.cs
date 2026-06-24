using Ads.Automation.Application.Contracts.Entity.Account;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Ads.Automation.Api.Controllers.Ads;

/// <summary>
/// 广告账户控制器
/// </summary>
[Route("api/ads/account")]
[ApiController]
public class AdsAccountController : ApiControllerBase
{
    private readonly IAdsAccountAppService _accountAppService;

    public AdsAccountController(IAdsAccountAppService accountAppService)
    {
        _accountAppService = accountAppService;
    }

    /// <summary>
    /// 获取账户信息
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(long id)
    {
        return Success(await _accountAppService.GetAsync(id));
    }

    /// <summary>
    /// 获取账户列表
    /// </summary>
    [HttpPost("list")]
    public async Task<IActionResult> GetListAsync([FromBody] GetAdsAccountListInput input)
    {
        return Success(await _accountAppService.GetListAsync(input));
    }

    /// <summary>
    /// 创建账户
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateAdsAccountDto input)
    {
        return Success(await _accountAppService.CreateAsync(input));
    }

    /// <summary>
    /// 修改账户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] CreateUpdateAdsAccountDto input)
    {
        return Success(await _accountAppService.UpdateAsync(id, input));
    }

    /// <summary>
    /// 删除账户
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _accountAppService.DeleteAsync(id);
        return Success<object?>(null);
    }
}
