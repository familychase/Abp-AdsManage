
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{

    public partial class MetaDomain
    {
        /// <summary>
        /// 广告组时间表类
        /// </summary>
        public class AdsetSchedule
        {
            /// <summary>
            /// 一个以0为基础的分钟，表示时间表开始的时间
            /// </summary>
            public long start_minute { get; set; }

            /// <summary>
            /// 以0为基础的一分钟，表示时间表结束的时间
            /// </summary>
            public long end_minute { get; set; }

            /// <summary>
            /// 表示日程活动的天数的整型数组。有效值为0-6,0代表Sunday, 1代表Monday，…及6代表星期六
            /// </summary>
            public List<long> days { get; set; } = null!;

            /// <summary>
            /// 默认值：USER
            /// enum {USER, ADVERTISER}
            /// </summary>
            public string timezone_type { get; set; } = null!;

        }

    }

}
