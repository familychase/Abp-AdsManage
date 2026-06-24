using Ads.Automation.Domain.Shared.Common;

namespace Ads.Automation.Application.Contracts.Entity.Users
{
    /// <summary>
    /// 用户登录结果Dto
    /// </summary>
    public class UserLoginResultDto
    {
        /// <summary>
        /// 登录Token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfoDto UserInfo { get; set; } = null!;
    }
}
