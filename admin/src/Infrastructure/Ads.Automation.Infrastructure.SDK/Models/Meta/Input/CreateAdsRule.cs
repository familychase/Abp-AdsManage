
/*
 * @author: alvin 2022-10-31 11:21
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{

    public partial class MetaInput
    {
        /// <summary>
        /// 创建广告规则.
        /// </summary>
        public class CreateAdsRule
        {
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
        }
    }
}
