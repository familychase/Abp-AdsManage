/*
 * @author: hujingtian 2024/12/30 10:43:08
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.CreativeAssetGroups
{
    /// <summary>
    /// 
    /// </summary>
    public class CreativeAssetGroupsTexts
    {
        /// <summary>
        /// 文案文本
        /// </summary>
        public string text { get; set; } = null!;

        /// <summary>
        /// 文案类型
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CreativeAssetGroupsTextType text_type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CreativeAssetGroupsTextType
    {
        /// <summary>
        /// 标题
        /// </summary>
        headline = 1,

        /// <summary>
        /// 正文
        /// </summary>
        primary_text = 2,

        /// <summary>
        /// 描述
        /// </summary>
        description = 3
    }

}
