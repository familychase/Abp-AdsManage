
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Output
{
    public partial class MetaOutput
    {
        public class IdResult : MetaErrorDto
        {
            /// <summary>
            /// id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// business_video_id
            /// </summary>
            public string? business_video_id { get; set; }
        }
    }
}
