using Ads.Automation.Application.Contracts.Entity.Users;
using Volo.Abp.Application.Dtos;

namespace Ads.Automation.Api.Controllers.System
{
    /// <summary>
    /// 用户信息控制器
    /// </summary>
    [Route("api/system/user")]
    [ApiController]
    public class UsersController : ApiControllerBase
    {
        private readonly ISysUserAppService _userAppService;
        private readonly UserInfoContext _userContext;

        public UsersController(ISysUserAppService userAppService, UserInfoContext userContext)
        {
            _userAppService = userAppService;
            _userContext = userContext;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(long id)
        {
            return Success(await _userAppService.GetAsync(id));
        }

        /// <summary>
        /// 获取用户列表信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("list")]
        public async Task<IActionResult> GetListAsync([FromBody] GetSysUserListInput input)
        {
            return Success(await _userAppService.GetListAsync(input));
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUpdateSysUserDto input)
        {
            return Success(await _userAppService.CreateAsync(input));
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, CreateUpdateSysUserDto input)
        {
            return Success(await _userAppService.UpdateAsync(id, input));
        }

        /// <summary>
        /// 修改个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}/self")]
        public async Task<IActionResult> UpdateSelfAsync(long id, UpdateSysUserSelfDto input)
        {
            return Success(await _userAppService.UpdateSelfAsync(id, input));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            return Success(await _userAppService.DeleteAsync(id));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/reset_pswd")]
        public async Task<IActionResult> ResetPasswordAsync(long id)
        {
            return Success(await _userAppService.ResetPasswordAsync(id));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("{id}/change_pswd")]
        public async Task<IActionResult> ChangePasswordAsync(long id, [FromBody] ChangeSysUserPasswordDto input)
        {
            return Success(await _userAppService.ChangePasswordAsync(id, input));
        }

        /// <summary>
        /// 获取当前登录用户信息（含权限标识和菜单树）
        /// </summary>
        /// <returns></returns>
        [HttpGet("self")]
        public async Task<IActionResult> GetSelfInfoAsync()
        {
            return Success(await _userAppService.GetSelfInfoAsync(_userContext.UserId));
        }

    }
}
