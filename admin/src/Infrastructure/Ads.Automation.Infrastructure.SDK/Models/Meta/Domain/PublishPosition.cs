/*
 * @author: hujingtian 2022-12-7 11:29:54
 */


// ReSharper disable InconsistentNaming
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 落地页详细信息
        /// </summary>
        public class PublishPosition
        {

            /// <summary>
            /// 版位发布的位置
            /// </summary>
            public List<string>? effective_publisher_platforms { get; set; }

            /// <summary>
            /// Facebook有效具体版位
            /// </summary>
            public List<string>? effective_facebook_positions { get; set; }

            /// <summary>
            /// instagram有效具体版位
            /// </summary>
            public List<string>? effective_instagram_positions { get; set; }

            /// <summary>
            /// messenger有效具体版位
            /// </summary>
            public List<string>? effective_messenger_positions { get; set; }

            /// <summary>
            /// audience_network有效具体版位
            /// </summary>
            public List<string>? effective_audience_network_positions { get; set; }

        }


    }
}
