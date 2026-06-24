
/*
 * @author: zhangwenjie 2023-08-22 18:03
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{

    public partial class MetaOutput
    {
        /// <summary>
        /// 事件上报结果
        /// </summary>
        public class EventReportedResult
        {
            /// <summary>
            /// 事件回收个数
            /// </summary>
            public string? events_received { get; set; }

            /// <summary>
            /// 消息列表
            /// </summary>
            public List<string>? messages { get; set; }
        }
    }
}
