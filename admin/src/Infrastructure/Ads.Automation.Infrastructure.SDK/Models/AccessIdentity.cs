
namespace Ads.Automation.Infrastructure.SDK.Models
{
    /// <summary>
    /// 访问身份.
    /// </summary>
    public class AccessIdentity
    {
        /// <summary>
        /// 
        /// </summary>
        public string AppKey { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string AppSecret { get; set; } = null!;

        /// <summary>
        /// 媒体经理级账户（统称）
        ///    Meta -> 经理账户。
        ///    Google -> MccId.
        /// </summary>
        public string ManagerId { get; set; } = null!;

        /// <summary>
        /// 媒体用户标识
        /// </summary>
        public string MediaIdentity { get; set; } = null!;

        /// <summary>
        /// 访问令牌.
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// 失效时间.
        /// </summary>
        public DateTime Expired { get; set; }

        /// <summary>
        /// 开发者token.
        /// </summary>
        public string DeveloperToken { get; set; } = null!;
    }


}
