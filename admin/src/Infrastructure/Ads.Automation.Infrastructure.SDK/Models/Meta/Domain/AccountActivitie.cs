
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 广告账户活动信息
        /// </summary>
        public class AccountActivity
        {
            /// <summary>
            /// 
            /// </summary>
            public string actor_id { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string? actor_name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? application_id { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string? date_time_in_timezone { get; set; }=null !;
            /// <summary>
            /// 
            /// </summary>
            public DateTime event_time { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string event_type { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string object_id { get; set; }= null!;
            /// <summary>
            /// {"old_value":"待审核","new_value":"未获批","campaign_id":23850734552360427,"type":"run_status"}
            /// </summary>
            public string? extra_data { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string? object_name { get; set; } = null!;
            /// <summary>
            /// 
            /// </summary>
            public string object_type { get; set; } = null!;

            /// <summary>
            /// 广告状态更新
            /// </summary>
            public string? translated_event_type { get; set; } = null!;

            /// <summary>
            /// 应用名称 -- 
            ///值为 Power Editor  对应Facebook
            /// </summary>
            public string? application_name { get; set; }
        }
    }
}
