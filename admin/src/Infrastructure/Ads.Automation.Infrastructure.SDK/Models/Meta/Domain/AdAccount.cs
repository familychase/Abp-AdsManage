
using System.Text.Json.Serialization;
using Ads.Automation.Infrastructure.SDK.Serialization;

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class AdAccount
        {
            /// <summary>
            /// Facebook系统广告账户ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 广告账户名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// Facebook系统广告账户ID
            /// </summary>
            public string account_id { get; set; } = null!;

            /// <summary>
            /// 状态
            /// </summary>
            public int account_status { get; set; }


            /// <summary>
            /// 失败原因类型
            /// </summary>
            public int disable_reason { get; set; }

            /// <summary>
            /// 余额
            /// </summary>
            public string? balance { get; set; }

            /// <summary>
            /// 帐户花费限额
            /// </summary>
            public string? spend_cap { get; set; }

            /// <summary>
            /// 已花费金额
            /// </summary>
            public string? amount_spent { get; set; }

            /// <summary>
            /// 货币
            /// </summary>
            public string? currency { get; set; }

            /// <summary>
            /// 时区名称
            /// </summary>
            public string? timezone_name { get; set; } = null!;

            /// <summary>
            /// 与 UTC 的时区差异
            /// </summary>
            public decimal timezone_offset_hours_utc { get; set; }

            /// <summary>
            /// 代理信息
            /// </summary>
            public Business? business { get; set; }

            /// <summary>
            /// 代理商信息
            /// </summary>
            public AgencyData? agencies { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            //[JsonConverter(typeof(DateTimeConverterUsingDateTimeParse))]
            public string? created_time { get; set; }

            /// <summary>
            /// 账户年龄
            /// </summary>
            public decimal? age { get; set; }
        }


        /// <summary>
        /// 账户代理数据
        /// </summary>
        public class AgencyData
        {
            /// <summary>
            /// 
            /// </summary>
            public List<Business>? data { get; set; }
        }


        /// <summary>
        /// 代理实体
        /// </summary>
        public class Business
        {
            /// <summary>
            /// 代理id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 代理名称
            /// </summary>
            public string name { get; set; } = null!;
        }

        /// <summary>
        /// 简约对象
        /// </summary>
        public class AdAccountSimple
        {
            /// <summary>
            /// API调用广告账户ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// Facebook系统广告账户ID
            /// </summary>
            public string account_id { get; set; } = null!;
        }
    }

}
