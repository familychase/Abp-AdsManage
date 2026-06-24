
/*
 * see : https://developers.facebook.com/docs/marketing-api/ad-rules/guides/trigger-based-rules
 * @author: alvin 2022-10-31 10:58
 */

// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class AdsRuleTriggerObject
        {
            /// <summary>
            /// The type of Trigger Based Rule. Current supported options are:
            /// 
            /// METADATA_CREATION: Triggers when an ad object is created
            /// 
            /// METADATA_UPDATE: Triggers when the metadata field is updated
            /// 
            /// STATS_CHANGE: Triggers when the insights field changes to satisfy the comparison
            /// 
            /// STATS_MILESTONE: Triggers when the insights field reaches a multiple of the value
            /// </summary>
            public string type { get; set; } = null!;

            /// <summary>
            /// The underlying field.Not in use for METADATA_CREATION
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string field { get; set; } = null!;

            /// <summary>
            /// The underlying filter value. Not in use for METADATA_CREATION. Optional for METADATA_UPDATE.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public object? value { get; set; } = null!;

            /// <summary>
            /// The underlying filter operator. Not in use for METADATA_CREATION. Optional for METADATA_UPDATE.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? @operator { get; set; } = null!;
        }
    }

}
