
/*
 * @author: zhangwenjie 2023-03-16 14:27
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaDomain
    {
        /// <summary>
        /// Token模型
        /// </summary>
        public class AccessToken
        {
            /// <summary>
            /// token
            /// </summary>
            public string access_token { get; set; } = null!;

            /// <summary>
            /// 类型
            /// </summary>
            public string? token_type { get; set; }

            /// <summary>
            /// 过期时间
            /// </summary>
            public int expires_in { get; set; }
        }
    }
}
