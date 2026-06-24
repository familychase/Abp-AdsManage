/*
 * @author: huangk  2022-12-7 19:06:19
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{


    public partial class MetaDomain
    {
        /// <summary>
        /// 回收值
        /// </summary>
        public class BidContraint
        {
            /// <summary>
            /// 回收值，与媒体相差10000倍
            /// </summary>
            public long roas_average_floor { get; set; }

        }

    }
}
