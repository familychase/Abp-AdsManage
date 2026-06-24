
/*
 * @author: zhangwenjie 2023-02-20 17:31
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 网域追踪
    /// </summary>
    public class MetaTrackingSpecs
    {
        /// <summary>
        /// 行为类型
        /// </summary>
        [JsonPropertyName("action.type")]
        public List<string> actionType { get; set; } = null!;

        /// <summary>
        /// 应用
        /// </summary>
        public List<string> application { get; set; } = null!;

        /// <summary>
        /// 应用
        /// </summary>
        public List<string> fb_pixel { get; set; } = null!;

        /// <summary>
        /// 主页，选择应用默认主页
        /// </summary>
        public List<string> page { get; set; } = null!;
    }
}
