namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// 用户登录登出Service接口
    /// </summary>
    public interface ISysUserLoginService : IApplicationService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="input">登录输入</param>
        /// <returns>登录结果（Token + 用户信息）</returns>
        Task<UserLoginResultDto> LoginAsync(UserLoginDto input);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="token">access_token值</param>
        Task LogoutAsync(string? token);
    }
}
