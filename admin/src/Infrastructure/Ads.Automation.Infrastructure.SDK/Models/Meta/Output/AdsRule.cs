
/*
 * @author:alvin 2022-10-31 11:34
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{
    public partial class MetaOutput
    {
        public class AdsRule
        {
            /// <summary>
            /// 广告规则Id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 广告规则名称
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 广告规则
            /// </summary>
            public MetaDomain.AdsRuleEvaluationSpec evaluation_spec { get; set; } = null!;

            /// <summary>
            /// 执行动作.
            /// </summary>
            public MetaDomain.AdsRuleExecutionSpec execution_spec { get; set; } = null!;

            /// <summary>
            /// 时间调度规则
            /// </summary>
            public MetaDomain.AdsRuleScheduleSpec? schedule_spc { get; set; }
        }
    }
    
}
