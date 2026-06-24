
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{

    public partial class MetaInput
    {
        public class CreateCustomAudience
        {
            /// <summary>
            /// 受众名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 受众规则
            /// </summary>
            public string? rule { get; set; }

            /// <summary>
            /// 类型
            /// </summary>
            public string? subtype { get; set; }

            /// <summary>
            /// 相似受众
            /// </summary>
            public string? lookalike_spec { get; set; }

            /// <summary>
            /// 相似受众来源来源
            /// </summary>
            public string? origin_audience_id { get; set; }
        }
    }
}
