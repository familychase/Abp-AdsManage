/*
 * @author: alvin 2022-10-24 11:46
 */
// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 轮播广告
    /// </summary>
    public class MetaChildAttachmentDto
    {
        /// <summary>
        /// 商店地址
        /// </summary>
        public string link { get; set; } = null!;

        /// <summary>
        /// 广告账户图像库中图像的哈希值
        /// </summary>
        public string image_hash { get; set; } = null!;

        /// <summary>
        /// 标题，用于模板广告
        /// </summary>
        public string name { get; set; } = null!;

        /// <summary>
        /// 标记为卡片，默认为true，用于模板广告
        /// </summary>
        public bool? static_card { get; set; }

        /// <summary>
        /// 模板广告的封面图片
        /// </summary>
        public string picture { get; set; }=null!;

        /// <summary>
        /// 模板广告的视频Id
        /// </summary>
        public string video_id { get; set; } = null!;

        /// <summary>
        /// 链接广告号召
        /// </summary>
        public MetaLinkAdCreativeCallToActionDto call_to_action { get; set; } = null!;
    }
}
