/*
 *@author: huangk 2022-12-8 10:18:52
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{

    public partial class MetaDomain
    {
        /// <summary>
        /// 视频结果类
        /// </summary>
        public class AdVideo
        {

            /// <summary>
            /// 视频Id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 视频临时地址，在应用建议上传素材时，获取账户视频临时地址
            /// </summary>
            public string source { get; set; } = null!;

            /// <summary>
            /// 视频永久地址
            /// https://www.facebook.com{permalink_url}
            /// </summary>
            public string permalink_url { get; set; } = null!;

            /// <summary>
            /// 时长
            /// </summary>
            public float length { get; set; }

            /// <summary>
            /// 标题
            /// </summary>
            public string title { get; set; } = null!;

            /// <summary>
            /// 更新时间
            /// </summary>
            public DateTime updated_time { get; set; }

            /// <summary>
            /// 视频缩略图
            /// </summary>
            public VideoThumbnails thumbnails { get; set; } = new VideoThumbnails();

            /// <summary>
            /// 视频状态
            /// </summary>
            public VideoStatus? status { get; set; }
        }

        /// <summary>
        /// 视频缩略图信息
        /// </summary>
        public class VideoThumbnails
        {
            /// <summary>
            /// 封面数据
            /// </summary>
            public List<ThumbnailInfo>? data { get; set; } = new List<ThumbnailInfo>();

        }

        /// <summary>
        /// 缩略图详细
        /// </summary>
        public class ThumbnailInfo
        {
            /// <summary>
            /// 封面id
            /// </summary>
            public string id { get; set; } = null!;

            /// <summary>
            /// 高度
            /// </summary>
            public int height { get; set; }

            /// <summary>
            /// 是否是首选
            /// </summary>
            public bool is_preferred { get; set; }

            /// <summary>
            /// 名字
            /// </summary>
            public string name { get; set; } = null!;

            /// <summary>
            /// 范围
            /// </summary>
            public float scale { get; set; }

            /// <summary>
            /// 封面图片地址
            /// </summary>
            public string uri { get; set; } = null!;

            /// <summary>
            /// 宽度
            /// </summary>
            public int width { get; set; }

        }

        /// <summary>
        /// 视频状态信息
        /// </summary>
        public class VideoStatus
        {
            /// <summary>
            /// Status of a video, either "ready" (uploaded, encoded, thumbnails extracted), "processing" (not ready yet) or "error" (processing failed).
            /// </summary>
            public string video_status { get; set; } = null!;
        }
    }
}
