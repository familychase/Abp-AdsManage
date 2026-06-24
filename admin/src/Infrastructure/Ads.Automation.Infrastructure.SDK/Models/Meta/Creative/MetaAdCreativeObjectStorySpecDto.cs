// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 广告创意规则（Object Story Spec）
    /// </summary>
    public class MetaAdCreativeObjectStorySpecDto
    {
        /// <summary>
        /// 主页ID，必填
        /// </summary>
        public string page_id { get; set; } = null!;

        /// <summary>
        /// Instagram 用户 ID（可选，用于 IG 绑定）
        /// </summary>
        public string? instagram_actor_id { get; set; }
        
        /// <summary>
        /// Instagram 用户ID，指定后广告将使用该 Instagram 账号作为发布身份。请注意，Instagram 用户必须与 Facebook 主页相关联，并且该主页必须具有管理员权限才能使用此功能。
        /// </summary>
        public string? instagram_user_id { get; set; }

        /// <summary>
        /// 链接页面帖子或传送带广告的规范
        /// </summary>
        public MetaLinkAdCreativeDto? link_data { get; set; }

        /// <summary>
        /// 在动态产品广告中使用的模板链接页面的规格
        /// </summary>
        public MetaLinkAdCreativeDto? template_data { get; set; }

        /// <summary>
        /// 文本类广告创意
        /// </summary>
        public MetaTextAdCreativeDto? text_data { get; set; }

        /// <summary>
        /// 图片类广告创意
        /// </summary>
        public MetaPhotoAdCreativeDto? photo_data { get; set; }

        /// <summary>
        /// 视频类广告创意
        /// </summary>
        public MetaVideoAdCreativeDto? video_data { get; set; }

        /// <summary>
        /// 动态广告多语言数据
        /// 按国家分发不同文案（如 "US"→英文文案, "FR"→法文文案）
        /// </summary>
        public List<DynamicAdCreativeDataItemDto>? dynamic_ad_creative_data { get; set; }
    }

    /// <summary>
    /// 动态广告多语言数据条目（dynamic_ad_creative_data）
    /// 按国家分发不同文案
    /// </summary>
    public class DynamicAdCreativeDataItemDto
    {
        /// <summary>
        /// 国家代码，如 "US"、"FR"、"CN"
        /// </summary>
        public string? country { get; set; }

        /// <summary>
        /// 广告正文
        /// </summary>
        public string? body { get; set; }

        /// <summary>
        /// 广告标题
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// 链接描述
        /// </summary>
        public string? link_description { get; set; }

        /// <summary>
        /// 行动号召文案
        /// </summary>
        public string? call_to_action_text { get; set; }

        /// <summary>
        /// 深度链接
        /// </summary>
        public string? deeplink_url { get; set; }
    }
}
