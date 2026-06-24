using Ads.Automation.Application.Contracts.Entity.Users;

namespace Ads.Automation.Api.Controllers
{
    /// <summary>
    /// Home controller
    /// </summary>
    [ApiController]
    public class HomeController : AbpController
    {
        private readonly ISysUserLoginService _loginService;
        private readonly IStringLocalizer<AdsAutomationResource> _localizer;

        public HomeController(
            ISysUserLoginService loginService,
            IStringLocalizer<AdsAutomationResource> localizer)
        {
            _loginService = loginService;
            _localizer = localizer;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/health")]
        public IActionResult GetAsync()
        {
            return Ok("success");
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/user/login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto input)
        {
            var result = await _loginService.LoginAsync(input);
            return Ok(WebApiResponse<object>.Success(result, _localizer["LoginSuccess"]));
        }

        /// <summary>
        /// User logout
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/user/login_out")]
        public async Task<IActionResult> LogoutAsync()
        {
            Request.Headers.TryGetValue("access_token", out var token);
            await _loginService.LogoutAsync(token);
            return Ok(WebApiResponse<object>.Success(null, _localizer["LogoutSuccess"]));
        }
    }
}
