
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// Meta - Ad campaign.
        /// </summary>
        public class AdCampaign
        {
            /// <summary>
            /// 广告系列ID
            /// </summary>
            public string? id { get; set; }

            /// <summary>
            /// 广告账户ID
            /// </summary>
            public string? account_id { get; set; }

            /// <summary>
            /// 出价策略
            /// 枚举：LOWEST_COST_WITHOUT_CAP，LOWEST_COST_WITH_BID_CAP，COST_CAP
            /// </summary>
            public string? bid_strategy { get; set; }

            /// <summary>
            /// 广告系列名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 状态
            /// enum {ACTIVE, PAUSED, DELETED, ARCHIVED}
            /// </summary>
            public string status { get; set; } = null!;

            /// <summary>
            /// 开始时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? start_time { get; set; }

            /// <summary>
            /// 停止时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? stop_time { get; set; }

            /// <summary>
            /// 更新时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? updated_time { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? created_time { get; set; }

            /// <summary>
            /// 每日预算
            /// </summary>
            public string? daily_budget { get; set; }

            /// <summary>
            /// 系列总预算
            /// </summary>
            public string? lifetime_budget { get; set; }

            /// <summary>
            /// 广告系列预算上限
            /// </summary>
            public string? spend_cap { get; set; }

            /// <summary>
            /// 目标 / 推广目标
            /// </summary>
            public string? objective { get; set; }

            /// <summary>
            /// 投放类型，枚举：standard、no_pacing
            /// </summary>
            public List<string> pacing_type { get; set; } = null!;

            /// <summary>
            /// 分类 枚举：NONE, EMPLOYMENT, HOUSING, CREDIT, ISSUES_ELECTIONS_POLITICS}
            /// </summary>
            public List<string> special_ad_categories { get; set; } = null!;

            /// <summary>
            /// 特殊分类选择地区
            /// </summary>
            public List<string>? special_ad_category_country { get; set; }

            /// <summary>
            /// 该字段将帮助 Facebook 优化交付、定价和限制。此广告系列中的所有广告组都必须与购买类型相匹配
            /// AUCTION（默认）、RESERVED（针对覆盖和频次广告）。
            /// </summary>
            public string buying_type { get; set; } = null!;

            /// <summary>
            /// 推广的对象
            /// </summary>
            public AdPromotedObject? promoted_object { get; set; }

            /// <summary>
            /// 智能促销类型，进阶赋能型，应用广告
            /// </summary>
            public string? smart_promotion_type { get; set; }

            /// <summary>
            /// 是否开启ios+ 14
            /// </summary>
            public bool? is_skadnetwork_attribution { get; set; }

            /// <summary>
            /// 异常信息
            /// </summary>
            public List<IssuesInfo>? issues_info { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public bool? can_use_spend_cap { get; set; }
        }
    }
}
