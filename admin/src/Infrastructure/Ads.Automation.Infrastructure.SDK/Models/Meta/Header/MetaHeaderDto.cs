
/*
 * @author: zhangwenjie 2023-05-04 15:53
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Header
{
    /// <summary>
    /// Meta 平台请求头模型
    /// </summary>
    public class MetaHeaderDto
    {
        /// <summary>
        /// x-business-use-case-usage中的账户限流模型
        /// </summary>
        public sealed class BusinessUserCaseUsageLimit
        {
            /// <summary>
            /// BUC限速逻辑类型
            /// </summary>
            public string type { get; set; } = null!;

            /// <summary>
            /// 调用的百分比
            /// </summary>
            public int call_count { get; set; }

            /// <summary>
            /// CPU已用总时间的百分比
            /// </summary>
            public int total_cputime { get; set; }

            /// <summary>
            /// 总已用时间的百分比
            /// </summary>
            public int total_time { get; set; }

            /// <summary>
            /// 重新获得访问的时间(以分钟为单位)
            /// </summary>
            public int estimated_time_to_regain_access { get; set; }
        }
    }
}
