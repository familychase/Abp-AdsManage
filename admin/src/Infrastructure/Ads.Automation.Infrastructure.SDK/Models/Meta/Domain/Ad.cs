
// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// Meta - Ad.
        /// </summary>
        public class Ad
        {
            /// <summary>
            /// 广告ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 广告名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 广告状态
            /// enum {ACTIVE, PAUSED, DELETED, ARCHIVED}
            /// </summary>
            public string status { get; set; }

            /// <summary>
            /// 广告有效的状态，DISAPPROVED
            /// </summary>
            public string? effective_status { get; set; }

            /// <summary>
            /// 出价金额
            /// </summary>
            public int? bid_amount { get; set; }

            /// <summary>
            /// 广告账户Id
            /// </summary>
            public string account_id { get; set; } = null!;

            /// <summary>
            /// 广告系列Id
            /// </summary>
            public string campaign_id { get; set; } = null!;

            /// <summary>
            /// 广告组Id
            /// </summary>
            public string adset_id { get; set; } = null!;

            /// <summary>
            /// 广告创建时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? created_time { get; set; }

            /// <summary>
            /// 广告更新时间
            /// </summary>
            [JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public DateTime? updated_time { get; set; }

        /// <summary>
        /// 广告创意信息（内嵌创意对象，二选一）
        /// </summary>
        public MetaCreativeDto? creative { get; set; }

        /// <summary>
        /// 广告创意 ID（引用已有创意，与 creative 二选一）
        /// 用于两步创建流程：先创建创意再创建广告
        /// </summary>
        public string? creative_id { get; set; }

            /// <summary>
            /// 网域与事件追踪
            /// </summary>
            public List<MetaTrackingSpecs>? tracking_specs { get; set; }

            /// <summary>
            /// 追踪域名
            /// </summary>
            public string conversion_domain { get; set; } = null!;

            /// <summary>
            /// 广告组信息
            /// </summary>
            public AdSet? adset { get; set; }

            /// <summary>
            /// 广告系列信息
            /// </summary>
            public AdCampaign? campaign { get; set; }

            /// <summary>
            /// 异常信息
            /// </summary>
            public List<IssuesInfo>? issues_info { get; set; }

            /// <summary>
            /// 灵活素材组
            /// </summary>
            public CreativeAssetGroupsSpec? creative_asset_groups_spec { get; set; }

            /// <summary>
            /// 修改广告状态
            /// </summary>
            public void SetStatus(string adStatus)
            {
                this.status = adStatus;
            }
        }


        /// <summary>
        /// Meta - 媒体异常信息
        /// </summary>
        public class IssuesInfo
        {
            /// <summary>
            /// 广告层级
            /// </summary>
            public string level { get; set; }

            /// <summary>
            /// 错误编码
            /// </summary>
            public int error_code { get; set; }

            /// <summary>
            /// 错误简称
            /// </summary>
            public string error_summary { get; set; }

            /// <summary>
            /// 错误详细信息
            /// </summary>
            public string error_message { get; set; }

            /// <summary>
            /// 错误类型
            /// </summary>
            public string error_type { get; set; }
        }
    }

}
