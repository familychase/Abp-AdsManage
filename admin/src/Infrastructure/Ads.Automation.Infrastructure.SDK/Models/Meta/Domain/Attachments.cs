
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// https://developers.facebook.com/docs/graph-api/reference/story-attachment/
        /// </summary>
        public class Attachments
        {
            /// <summary>
            /// 附件中包含的媒体对象（照片、链接等）
            /// </summary>
            public AttachmentsMedia media { get; set; } = null!;

            /// <summary>
            /// 媒体类型，如（照片、视频、链接等）
            /// </summary>
            public string media_type { get; set; }

            /// <summary>
            /// 附件链接到的对象
            /// </summary>
            public AttachmentsTarget target { get; set; } = null!;

            /// <summary>
            /// Type of the attachment.Possible types include: album, animated_image_autoplay, checkin, cover_photo, event, link, multiple, music, note, offer, photo, profile_media, status, video, video_autoplay, etc.
            /// </summary>
            public string type { get; set; } = null!;

            /// <summary>
            /// 附件标题
            /// </summary>
            public string title { get; set; } = null!;

            /// <summary>
            /// 附件的 URL
            /// </summary>
            public string url { get; set; } = null!;

            /// <summary>
            /// 附件中的文字
            /// </summary>
            public string description { get; set; }
        }

        /// <summary>
        /// 附件中包含的媒体对象（照片、链接等）
        /// </summary>
        public class AttachmentsMedia
        {
            /// <summary>
            /// 附件中包含的媒体对象（照片、链接等）
            /// </summary>
            public AttachmentsImage image { get; set; } = null!;

            /// <summary>
            /// 地址来源
            /// </summary>
            public string source { get; set; } = null!;
        }

        /// <summary>
        /// 
        /// </summary>
        public class AttachmentsImage
        {
            /// <summary>
            /// 高
            /// </summary>
            public int height { get; set; }

            /// <summary>
            /// 宽
            /// </summary>
            public int width { get; set; }

            /// <summary>
            /// 地址
            /// </summary>
            public string src { get; set; } = null!;
        }

        /// <summary>
        /// 
        /// </summary>
        public class AttachmentsTarget
        {
            /// <summary>
            /// ID
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 网址
            /// </summary>
            public string url { get; set; } = null!;
        }

    }
}
