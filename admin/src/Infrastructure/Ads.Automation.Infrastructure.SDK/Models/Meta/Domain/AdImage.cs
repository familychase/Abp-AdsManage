
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Domain
{
    public partial class MetaDomain
    {
        /// <summary>
        /// 广告图片信息
        /// </summary>
        public class AdImage
        {

            /// <summary>
            /// 图片HASH值
            /// </summary>
            public string hash { get; set; } = null!;

            /// <summary>
            /// 高度
            /// </summary>
            public int height { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            public string? name { get; set; }

            /// <summary>
            /// 永久的URL
            /// </summary>
            public string permalink_url { get; set; } = null!;

            /// <summary>
            /// 一个临时URL，指向调整大小以适应128x128像素框的图像版本
            /// </summary>
            public string url_128 { get; set; } = null!;

            /// <summary>
            /// 宽度
            /// </summary>
            public int width { get; set; } 
        }
    }
}
