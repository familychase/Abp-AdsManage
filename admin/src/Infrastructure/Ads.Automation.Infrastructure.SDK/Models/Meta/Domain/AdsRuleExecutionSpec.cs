
/*
 * see : https://developers.facebook.com/docs/marketing-api/ad-rules/overview/execution-spec
 * @author: alvin 2022-10-31 11:11
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class AdsRuleExecutionSpec
        {
            public string execution_type { get; set; } = null!;
        }
    }
}
