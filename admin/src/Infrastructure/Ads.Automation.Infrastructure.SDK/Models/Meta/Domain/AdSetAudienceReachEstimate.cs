
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        public class AdSetAudienceReachEstimate
        {
            public AudienceReachEstimate data { get; set; }
        }

        public class AudienceReachEstimate
        {
            public long users_lower_bound { get; set; }

            public long users_upper_bound { get; set; }

            public bool estimate_ready { get; set; }
        }
    }
}
