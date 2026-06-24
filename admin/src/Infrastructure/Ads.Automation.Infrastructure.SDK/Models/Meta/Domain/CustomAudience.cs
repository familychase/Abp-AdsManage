/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class CustomAudience
        {
            /// <summary>
            /// 受众Id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 受众名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 受众类型：ENGAGEMENT、APP、LOOKALIKE
            /// </summary>
            public string? subtype { get; set; }

            /// <summary>
            /// 受众规则，当前为主页、应用信息
            /// </summary>
            public string? rule { get; set; }

            /// <summary>
            /// 类似受众
            /// </summary>
            public LookalikeInfo? lookalike_spec { get; set; }

            /// <summary>
            /// 类似受众详情
            /// </summary>
            public class LookalikeInfo
            {

                /// <summary>
                /// 类似受众来源
                /// </summary>
                public List<LookalikeOriginInfo>? origin { get; set; }

                /// <summary>
                /// 类似受众来源
                /// </summary>
                public class LookalikeOriginInfo
                {
                    /// <summary>
                    /// 受众Id
                    /// </summary>
                    public string id { get; set; } = null!;
                    /// <summary>
                    /// 受众名称
                    /// </summary>
                    public string name { get; set; } = null!;

                }

                /// <summary>
                /// 受众定位国家列表
                /// </summary>
                public List<string>? target_countries { get; set; }

                /// <summary>
                /// 类似受众类型
                /// </summary>
                public string? type { get; set; }
            }

            /// <summary>
            /// 此受众中大约人数的下限
            /// </summary>
            public long approximate_count_lower_bound { get; set; }

            /// <summary>
            /// 此受众中大约人数的上限
            /// </summary>
            public long approximate_count_upper_bound { get; set; }

            /// <summary>
            /// 状态，只有code为200时有效
            /// </summary>
            public DeliveryStatus? delivery_status { get; set; }

            /// <summary>
            /// 状态模型
            /// </summary>
            public class DeliveryStatus
            {
                /// <summary>
                /// 编码
                /// </summary>
                public long code { get; set; }

            }


        }
    }
}
