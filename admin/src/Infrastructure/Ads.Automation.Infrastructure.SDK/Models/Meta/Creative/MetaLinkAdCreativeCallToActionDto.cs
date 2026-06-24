/*
 * @author: alvin 2022-10-24 11:43
 */

// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 广告创意 - 号召行用语
    /// </summary>
    public class MetaLinkAdCreativeCallToActionDto
    {
        /// <summary>
        /// 行动号召
        /// </summary>
        public string type { get; set; } = null!;

        /// <summary>
        /// 包含号召性用语数据的 JSON
        /// </summary>
        public MetaLinkAdCreativeCallToActionValueDto? value { get; set; }
    }


    /// <summary>
    /// Meta - 广告创意 - 号召行用语数据
    /// </summary>
    public record MetaLinkAdCreativeCallToActionValueDto
    {
        /// <summary>
        /// 单击 CTA 按钮时的目标链接。这必须与广告素材的链接网址相同。
        /// </summary>
        public string link { get; set; } = null!;

        /// <summary>
        /// 视频的显示地址
        /// </summary>
        public string link_caption { get; set; } = null!;

        /// <summary>
        /// 应用程序的深层链接
        /// </summary>
        public string app_link { get; set; } = null!;
    }

}
