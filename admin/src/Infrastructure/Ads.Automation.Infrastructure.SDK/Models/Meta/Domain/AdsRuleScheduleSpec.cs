
/*
 * See: https://developers.facebook.com/docs/marketing-api/ad-rules/guides/scheduled-based-rules
 * @author: alvin 2022-10-31 11:16
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// Monitor the state of your ads by checking them at a set interval to see if they meet the evaluation_spec criteria. For Schedule Based Rules, an additional schedule_spec is required.
        /// </summary>
        public class AdsRuleScheduleSpec
        {
            /// <summary>
            /// The schedule_spec of a rule determines how frequently you want it to run. 
            /// </summary>
            public string schedule_type { get; set; } = null!;
        }
    }
}
