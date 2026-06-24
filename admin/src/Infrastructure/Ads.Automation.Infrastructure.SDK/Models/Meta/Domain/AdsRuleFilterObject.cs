
/*
 * see : https://developers.facebook.com/docs/marketing-api/ad-rules/overview/evaluation-spec
 * @author: alvin 2022-10-31 11:03
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{

    public partial class MetaDomain
    {
        /// <summary>
        /// The filters array contains a list of filter objects.
        /// </summary>
        public class AdsRuleFilterObject
        {
            /// <summary>
            /// Required.
            /// Filter field, such as Insights data or metadata
            /// </summary>
            public string field { get; set; } = null!;

            /// <summary>
            /// Required.
            /// Static filter value for the field
            /// </summary>
            public string value { get; set; } = null!;

            /// <summary>
            /// Required.
            /// Logical operator for the field
            /// </summary>
            public string @operator { get; set; } = null!;
        }
    }
}
