
/*
 * @author: alvin 2022-10-31 10:51
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// The main purpose of the evaluation_spec of a rule is to determine the objects upon which the rule should execute its action.
        /// </summary>
        public class AdsRuleEvaluationSpec
        {
            /// <summary>
            /// 规则类型
            /// </summary>
            public string evaluation_type { get; set; } = null!;

            /// <summary>
            /// 触发器规则.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public AdsRuleTriggerObject? trigger { get; set; }

            /// <summary>
            /// 过滤器
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]

            public List<AdsRuleFilterObject>? filters { get; set; }

            /// <summary>
            /// 时间调度规则.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public AdsRuleScheduleSpec? schedule_spec { get; set; }
        }
    }
}
