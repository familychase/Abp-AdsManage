/*
 * @author: hujingtian 2024/12/30 10:59:52
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.CreativeAssetGroups
{
    /// <summary>
    /// 
    /// </summary>
    public class CreativeAssetGroupsVideos
    {
        /// <summary>
        /// 视频id
        /// </summary>
        public string video_id { get; set; } = null!;

        /// <summary>
        /// 视频缩略图hash
        /// </summary>
        public string? image_hash { get; set; } = null!;

        /// <summary>
        /// 缩略图来源
        /// </summary>
        public string? thumbnail_source { get; set; } = null!;
    }

}
