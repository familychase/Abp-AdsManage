/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class Interest
        {
            /// <summary>
            /// 兴趣id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 兴趣名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 受众范围下限
            /// </summary>
            public long audience_size_lower_bound { get; set; }

            /// <summary>
            /// 受众范围上限
            /// </summary>
            public long audience_size_upper_bound { get; set; }

            /// <summary>
            /// 兴趣上级导航
            /// </summary>
            public List<string>? path { get; set; }

            /// <summary>
            /// 兴趣描述
            /// </summary>
            public string? description { get; set; }

            /// <summary>
            /// 主题
            /// </summary>
            public string? topic { get; set; }
        }

    }
}
