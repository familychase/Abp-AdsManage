
/*
 * @author: alvin 2022-11-4 19:07
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// Meta - 报告
        /// </summary>
        public class Reporting
        {
            /// <summary>
            /// 广告账户Id
            /// </summary>
            public string account_id { get; set; } = null!;

            /// <summary>
            /// 广告系列Id
            /// </summary>
            public string? campaign_id { get; set; }

            /// <summary>
            /// 广告系列名称
            /// </summary>
            public string? campaign_name { get; set; }

            /// <summary>
            /// 广告组Id
            /// </summary>
            public string? adset_id { get; set; }

            /// <summary>
            /// 广告Id
            /// </summary>
            public string? ad_id { get; set; }

            /// <summary>
            /// 点击数
            /// </summary>
            public string? clicks { get; set; }

            /// <summary>
            /// 展示数
            /// </summary>
            public string? impressions { get; set; }

            /// <summary>
            /// 花费
            /// </summary>
            public string? spend { get; set; }

            /// <summary>
            /// 开始时间
            /// </summary>
            //[JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public string? date_start { get; set; }

            /// <summary>
            /// 小时的范围
            /// </summary>
            public string? hourly_stats_aggregated_by_advertiser_time_zone { get; set; }

            /// <summary>
            /// 国家
            /// </summary>
            public string? country { get; set; }

            /// <summary>
            /// 性别
            /// </summary>
            public string? gender { get; set; }

            /// <summary>
            /// 年龄
            /// </summary>
            public string? age { get; set; }

            /// <summary>
            /// 发布平台
            /// </summary>
            public string? publisher_platform { get; set; }

            /// <summary>
            /// 平台版位
            /// </summary>
            public string? platform_position { get; set; }

            /// <summary>
            /// 展示设备
            /// </summary>
            public string? impression_device { get; set; }

            /// <summary>
            /// 转化事件
            /// </summary>
            public List<ReportingAction>? actions { get; set; }

            /// <summary>
            /// 转化事件价值
            /// </summary>
            public List<ReportingAction>? action_values { get; set; }

            /// <summary>
            /// 网站的转化事件
            /// </summary>
            public List<ReportingAction>? conversions { get; set; }

            /// <summary>
            /// 网站的转化事件价值
            /// </summary>
            public List<ReportingAction>? conversion_values { get; set; }

            /// <summary>
            /// 视频的平均播放时长
            /// </summary>
            public List<ReportingAction>? video_avg_time_watched_actions { get; set; }

            /// <summary>
            /// 视频播放75%以上的次数
            /// </summary>
            public List<ReportingAction>? video_p75_watched_actions { get; set; }

            /// <summary>
            /// 视频播放2S以上的次数
            /// </summary>
            public List<ReportingAction>? video_continuous_2_sec_watched_actions { get; set; }

            /// <summary>
            /// 视频资产信息
            /// </summary>
            public ReportingVideoAsset? video_asset { get; set; }

            /// <summary>
            /// 图片资产信息
            /// </summary>
            public ReportingImageAsset? image_asset { get; set; }

            /// <summary>
            /// BI上的转化数，用于报表的统计
            /// </summary>
            public decimal? BiConverts { get; set; }

            /// <summary>
            /// 应用Id（冗余字段，接口不一定返回）
            /// </summary>
            public string? ApplicationId { get; set; } = null!;

            /// <summary>
            /// 像素Id（冗余字段，接口不一定返回）
            /// </summary>
            public string? PixelId { get; set; }
        }


        public class ReportingAction
        {
            /// <summary>
            /// 
            /// </summary>
            public string action_type { get; set; } = null!;

            /// <summary>
            /// 默认归因窗口的指标值
            /// </summary>
            public string? value { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("1d_click")]
            public string? click_1d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("7d_click")]
            public string? click_7d { get; set; }

            /// <summary>
            /// 点击后 28 天
            /// </summary>
            [JsonPropertyName("28d_click")]
            public string? click_28d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("1d_view")]
            public string? view_1d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("7d_view")]
            public string? view_7d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("28d_view")]
            public string? view_28d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("1d_ev")]
            public string? ev_1d { get; set; }

        }

        /// <summary>
        /// 
        /// </summary>
        public class ReportingConversion
        {
            /// <summary>
            /// 
            /// </summary>
            public string action_type { get; set; } = null!;

            /// <summary>
            /// 默认归因窗口的指标值
            /// </summary>
            public string? value { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("1d_click")]
            public string? click_1d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("7d_click")]
            public string? click_7d { get; set; }

            /// <summary>
            /// 点击后 28 天
            /// </summary>
            [JsonPropertyName("28d_click")]
            public string? click_28d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("1d_view")]
            public string? view_1d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("7d_view")]
            public string? view_7d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("28d_view")]
            public string? view_28d { get; set; }

            /// <summary>
            /// 
            /// </summary>
            [JsonPropertyName("1d_ev")]
            public string? ev_1d { get; set; }
        }

        /// <summary>
        /// 报表的视频资产信息
        /// </summary>
        public class ReportingVideoAsset
        {
            /// <summary>
            /// 视频id
            /// </summary>
            public string video_id { get; set; } = null!;
        }

        /// <summary>
        /// 报表的事情资产信息
        /// </summary>
        public class ReportingImageAsset
        {
            /// <summary>
            /// 图片hash值
            /// </summary>
            public string hash { get; set; } = null!;
        }

    }
}
