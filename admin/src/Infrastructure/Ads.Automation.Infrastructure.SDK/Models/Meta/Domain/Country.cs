/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class Country
        {  /// <summary>
           /// 国家key值，区域与城市投放时使用
           /// </summary>
            public string? key { get; set; }

            /// <summary>
            /// 区域|城市名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 国家类型：REGION：区域，city：城市
            /// </summary>
            public string? type { get; set; }

            /// <summary>
            /// 国家名称
            /// </summary>
            public string? country_code { get; set; }

            /// <summary>
            /// 国家名称
            /// </summary>
            public string? country_name { get; set; }

            /// <summary>
            /// 区域名称
            /// </summary>
            public string? region { get; set; }

            /// <summary>
            /// 区域代码
            /// </summary>
            public long? region_id { get; set; }

            /// <summary>
            /// 拥有区域代码
            /// </summary>
            public bool supports_region { get; set; }

            /// <summary>
            /// 拥有城市代码
            /// </summary>
            public bool supports_city { get; set; }

            /// <summary>
            /// 国家组编码
            /// </summary>
            public List<string>? country_codes { get; set; }
        }
    }

}
