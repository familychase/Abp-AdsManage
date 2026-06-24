/*
 * @author: hujingtian 2024/12/30 10:40:19
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.CreativeAssetGroups
{
    /// <summary>
    /// 
    /// </summary>
    public class CreativeAssetGroupsSpec
    {
        public List<CreativeAssetGroups> groups { get; set; } = null!;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CreativeAssetGroups
    {
        /// <summary>
        /// 行动号召
        /// </summary>
        public MetaLinkAdCreativeCallToActionDto? call_to_action { get; set; }

        /// <summary>
        /// 文案列表
        /// </summary>
        public List<CreativeAssetGroupsTexts>? texts { get; set; }

        /// <summary>
        /// 视频信息
        /// </summary>
        public List<CreativeAssetGroupsVideos>? videos { get; set; }

        /// <summary>
        /// 图片信息
        /// </summary>
        public List<CreativeAssetGroupsImages>? images { get; set; }
    }


}
