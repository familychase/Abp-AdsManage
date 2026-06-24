
/*
 * @author: alvin 2022-10-25 19:29
 */

// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// Meta - 归因设置.
        /// </summary>
        public class AttributionSpec
        {
            public AttributionSpec() { }

            /// <summary>
            /// 事件类型
            /// enum {CLICK_THROUGH, VIEW_THROUGH}
            /// </summary>
            public string event_type { get; set; } = null!;

            /// <summary>
            /// 
            /// </summary>
            public long window_days { get; set; }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="eType"></param>
            /// <param name="wDay"></param>
            public  AttributionSpec(string eType,string wDay)
            {
                this.event_type = eType;
                this.window_days = Convert.ToInt64(wDay);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="eType"></param>
            /// <param name="wDay"></param>
            public AttributionSpec(string eType, Int64 wDay)
            {
                this.event_type = eType;
                this.window_days = wDay;
            }
        }
    }
}
