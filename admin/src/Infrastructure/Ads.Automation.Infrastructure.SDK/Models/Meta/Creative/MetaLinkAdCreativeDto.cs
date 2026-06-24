/*
 * @author: alvin 2022-10-24 11:40
 */

// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 广告创意（链接类）
    /// </summary>
    public class MetaLinkAdCreativeDto
    {
        /// <summary>
        /// 广告账户图像库中图像的哈希值。为该字段或“图片”提供一个值，但不能同时提供两者
        /// </summary>
        public string? image_hash { get; set; }

        /// <summary>
        /// 链接网址。此 url 必须与 CTA 链接 url 相同。
        /// </summary>
        public string link { get; set; } = null!;

        /// <summary>
        /// 显示地址
        /// </summary>
        public string? caption { get; set; }

        /// <summary>
        /// 链接的名称
        /// </summary>
        public string? name { get; set; } = null!;

        /// <summary>
        /// 帖子的主体
        /// </summary>
        public string? message { get; set; } = null!;

        /// <summary>
        /// 描述
        /// </summary>
        public string? description { get; set; } = null!;

        /// <summary>
        /// 是否为幻灯片，true，传入carousel_slideshows
        /// </summary>
        public string format_option { get; set; } = null!;

        /// <summary>
        /// 是否使用最后一张图片，默认为false，校验必传
        /// </summary>
        public bool? multi_share_end_card { get; set; }

        /// <summary>
        /// 链接广告号召
        /// </summary>
        public MetaLinkAdCreativeCallToActionDto? call_to_action { get; set; }

        /// <summary>
        /// 轮播广告
        /// </summary>
        public List<MetaChildAttachmentDto>? child_attachments { get; set; }

        /// <summary>
        /// 是否共享优化
        /// </summary>
        public bool? multi_share_optimized { get; set; }

        /// <summary>
        /// 与force_single_link = true一起使用，以便使用目录中的多个图像以轮播格式显示单个动态项目。查看动态产品广告
        /// </summary>
        public bool? show_multiple_images { get; set; }

        /// <summary>
        /// 是否强制帖子以单个链接格式呈现
        /// </summary>
        public bool? force_single_link { get; set; }
    }
}
