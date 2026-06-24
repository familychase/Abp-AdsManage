

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class SubdivisionTargetingSearch
        {
            /// <summary>
            /// 目标受众的编号
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 目标受众的名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            ///   目标受众规模的预估值下限
            /// </summary>
            public long audience_size_lower_bound { get; set; }

            /// <summary>
            /// 目标受众规模的预估值上限
            /// </summary>
            public long audience_size_upper_bound { get; set; }

            /// <summary>
            ///  包括定位隶属的类别及任何父类别
            /// </summary>
            public List<string> path { get; set; } = null!;

            /// <summary>
            /// 有关目标受众的简短描述
            /// </summary>
            public string description { get; set; } = null!;

            /// <summary>
            /// 细分定位类型
            /// </summary>
            public string type { get; set; }
        }

    }
}
