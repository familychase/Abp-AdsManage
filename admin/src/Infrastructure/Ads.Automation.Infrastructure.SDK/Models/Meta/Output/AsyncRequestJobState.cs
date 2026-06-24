
/*
 * @author: alvin 2022-11-5 11:02
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{
    public partial class MetaOutput
    {
        /// <summary>
        /// 异步请求状态
        /// </summary>
        public class AsyncRequestJobState
        {
            public string? id { get; set; }

            /// <summary>
            /// 账户ID
            /// </summary>
            public string account_id { get; set; } = null!;

            public int time_ref { get; set; }

            public int time_completed { get; set; }

            public string async_status { get; set; } = null!;

            public int async_percent_completion { get; set; }
        }
    }
    
}
