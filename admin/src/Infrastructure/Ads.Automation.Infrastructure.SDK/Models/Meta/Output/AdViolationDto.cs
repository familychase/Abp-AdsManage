/*
 * @author huangk  2022-12-7 17:57:47
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{
    public partial class MetaOutput
    {
        /// <summary>
        /// 广告违规原因信息
        /// </summary>
        public class AdViolationDto
        {
            /// <summary>
            /// 是否违规
            /// </summary>
            public bool IsViolation { get; set; }

            /// <summary>
            /// 违规原因
            /// </summary>
            public string ViolationReason { get; set; } = null!;

        }
    }
}
