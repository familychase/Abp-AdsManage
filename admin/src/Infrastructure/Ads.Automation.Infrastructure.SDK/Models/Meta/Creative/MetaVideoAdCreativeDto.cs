/*
 * @author: alvin 2022-10-24 11:54
 */

// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 广告创意 - 视频类
    /// </summary>
    public class MetaVideoAdCreativeDto
    {
        /// <summary>
        /// 用作缩略图的图像的 URL。您不应使用从 FB CDN 返回的图像 URL，而是将图像托管在您自己的服务器上。
        /// </summary>
        public string image_url { get; set; } = null!;

        /// <summary>
        /// 用作缩略图的图像的 URL。您不应使用从 FB CDN 返回的图像 URL，而是将图像托管在您自己的服务器上。
        /// </summary>
        public string image_hash { get; set; } = null!;

        /// <summary>
        /// 视频帖子的主体
        /// </summary>
        public string? message { get; set; } = null!;

        /// <summary>
        /// 视频的标题。这不能与LIKE_PAGE号召性用语一起使用。
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// 用户有权限的视频ID或广告账号视频库中的视频。
        /// </summary>
        public string video_id { get; set; } = null!;

        /// <summary>
        /// 描述
        /// </summary>
        public string? link_description { get; set; } = null!;

        /// <summary>
        /// 一个可选的号召性用语。此外，您可以LIKE_PAGE在广告位于 PAGE_LIKES 广告系列中时指定号召性用语
        /// </summary>
        public MetaLinkAdCreativeCallToActionDto? call_to_action { get; set; }
    }
}
