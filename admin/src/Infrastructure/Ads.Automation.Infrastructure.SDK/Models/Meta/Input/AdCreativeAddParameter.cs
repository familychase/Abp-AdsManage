/*
 * @author: huangk 2022-12-7 18:41:43
 */

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Input
{

    public partial class MetaInput
    {
        /// <summary>
        /// 新增广告创意
        /// </summary>
        public class AdCreativeAddParameter
        {

            /// <summary>
            /// 创意名称
            /// </summary>
            public string name { get; set; }=null!;

            /// <summary>
            /// 创意
            /// </summary>
            public MetaAdCreativeObjectStorySpecDto object_story_spec { get; set; } = null!;

            /// <summary>
            /// 创意美化
            /// </summary>
            public DegreesOfFreedomSpecDto? degrees_of_freedom_spec { get; set; }

            /// <summary>
            /// 动态广告创意
            /// </summary>
            public MetaAdAssetFeedSpecDto? asset_feed_spec { get; set; }

            /// <summary>
            /// 商品系列Id，必填
            /// </summary>
            public string? product_set_id { get; set; } = null!;

            /// <summary>
            /// 模板广告的深度链接追踪
            /// </summary>
            public MetaTemplateAdCreativeDto? template_url_spec { get; set; } = null!;

            /// <summary>
            /// 使用公共主页而不是应用名称作为广告发布身份
            /// </summary>
            public bool? use_page_actor_override { get; set; }

            /// <summary>
            /// 指定广告是否配置为标记为政治广告。请参阅Facebook 广告政策。该字段不能用于动态广告。
            /// </summary>
            public string? authorization_category { get; set; }

            /// <summary>
            /// 区域监管免责声明创意创作规范
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public RegionalRegulationDisclaimerSpec? regional_regulation_disclaimer_spec { get; set; }
        }
    }
}
