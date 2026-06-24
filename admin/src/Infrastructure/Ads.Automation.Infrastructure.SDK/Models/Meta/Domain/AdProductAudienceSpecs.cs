
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class AdProductAudienceSpecs
        {
            /// <summary>
            /// 商品目录的系列Id
            /// </summary>
            public string product_set_id { get; set; } = null!;

            /// <summary>
            /// 包含受众列表
            /// </summary>
            public List<AudienceSpecs>? inclusions { get; set; }

            /// <summary>
            /// 排除受众列表
            /// </summary>
            public List<AudienceSpecs>? exclusions { get; set; }

            /// <summary>
            /// 受众信息详情
            /// </summary>
            public class AudienceSpecs
            {
                /// <summary>
                /// 时间（s）
                /// </summary>
                public string retention_seconds { get; set; } = null!;

                /// <summary>
                /// 受众的规则信息
                /// </summary>
                public string rule { get; set; } = null!;
            }
        }
    }
}
