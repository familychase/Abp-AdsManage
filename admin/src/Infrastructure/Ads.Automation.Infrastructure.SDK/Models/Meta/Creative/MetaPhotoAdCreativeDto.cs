/*
 * @author: alvin 2022-10-24 11:51
 */

// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 广告创意 - 图片类
    /// </summary>
    public class MetaPhotoAdCreativeDto
    {
        /// <summary>
        /// 品牌内容分享给赞助商选项
        /// </summary>
        public string? branded_content_shared_to_sponsor_status { get; set; }

        /// <summary>
        /// 品牌内容赞助商页面 ID。
        /// </summary>
        public string? branded_content_sponsor_page_id { get; set; }

        /// <summary>
        /// 品牌内容赞助商关系选项
        /// </summary>
        public string? branded_content_sponsor_relationship { get; set; }

        /// <summary>
        /// 图片说明
        /// </summary>
        public string? caption { get; set; }

        /// <summary>
        /// 使用 Facebook 对图像库中的图像进行哈希处理。指定此字段或url但不指定两者。
        /// </summary>
        public string image_hash { get; set; } = null!;

        /// <summary>
        /// 用户对广告执行发送消息操作后，Messenger 页面上向用户发送的欢迎文本
        /// </summary>
        public string? page_welcome_message { get; set; }

        /// <summary>
        /// 要在广告中使用的图片的网址。指定此字段或image_hash但不指定两者。
        /// </summary>
        public string? url { get; set; }
    }
}
