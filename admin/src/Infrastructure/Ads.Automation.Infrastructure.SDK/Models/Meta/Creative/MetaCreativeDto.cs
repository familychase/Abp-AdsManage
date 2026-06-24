
namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 广告创意.
    /// </summary>
    public class MetaCreativeDto
    {
        /// <summary>
        /// 创意 id
        /// </summary>
        public string creative_id { get; set; } = null!;

        /// <summary>
        /// 创意 id
        /// </summary>
        public string id { get; set; } = null!;

        /// <summary>
        /// 广告中使用的 Facebook 主页帖子的 ID
        /// 。您可以通过查询主页的帖子获取此 ID 。
        /// 如果此帖子包含图片，则图片大小不得超过 8 MB。
        /// Facebook 会将帖子中的图片上传到您的广告帐户的图片库object_story_spec。
        /// 如果您在创建广告的同时通过创建未发布的主页帖子，
        /// 此 ID 将为空。但是，effective_object_story_id无论是自然发布还是未发布的主页帖子，都将是主页帖子的 ID。
        /// </summary>
        public string? object_story_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MetaAdCreativeObjectStorySpecDto? object_story_spec { get; set; }

        /// <summary>
        /// 动态广告创意
        /// </summary>
        public MetaAdAssetFeedSpecDto? asset_feed_spec { get; set; }

        /// <summary>
        /// 商品系列Id，必填
        /// </summary>
        public string product_set_id { get; set; } = null!;

        /// <summary>
        /// 模板广告的深度链接追踪
        /// </summary>
        public MetaTemplateAdCreativeDto template_url_spec { get; set; } = null!;

        /// <summary>
        /// 使用公共主页而不是应用名称作为广告发布身份
        /// </summary>
        public bool? use_page_actor_override { get; set; }

        /// <summary>
        /// 公共主页身份id
        /// </summary>
        public string? actor_id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DegreesOfFreedomSpecDto? degrees_of_freedom_spec { get; set; }

        /// <summary>
        /// 指定广告是否配置为标记为政治广告。请参阅Facebook 广告政策。该字段不能用于动态广告。
        /// </summary>
        public string? authorization_category { get; set; }


        /// <summary>
        /// 广告创意上下文多广告
        /// </summary>
        public StandardEnhancementsDto contextual_multi_ads { get; set; } = null!;

        /// <summary>
        /// 区域监管免责声明创意创作规范
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RegionalRegulationDisclaimerSpec? regional_regulation_disclaimer_spec { get; set; }
    }

    /// <summary>
    /// 区域监管免责声明创意创作规范
    /// regional_regulation_disclaimer_spec
    /// 用于为您的广告素材指定区域监管免责声明。请传递您要创建的广告素材的相关字段。
    /// </summary>
    public class RegionalRegulationDisclaimerSpec
    {
        /// <summary>
        /// 台湾金融服务（Finserv）
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TaiwanFinserv? taiwan_finserv { get; set; }

        /// <summary>
        /// 澳大利亚金融服务公司（Finserv）
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AustraliaFinserv? australia_finserv { get; set; }

        /// <summary>
        /// 台湾环球
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TaiwanUniversal? taiwan_universal { get; set; }

        /// <summary>
        /// 新加坡环球
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SingaporeUniversal? singapore_universal { get; set; }
    }

    /// <summary>
    /// 新加坡环球
    /// </summary>
    public class SingaporeUniversal
    {
        public SingaporeUniversal()
        {
        }

        public SingaporeUniversal(string? beneficiary_id, string? finserv_funder_id)
        {
            this.singapore_universal_beneficiary_id = beneficiary_id;
            this.singapore_universal_payer_id = finserv_funder_id;
        }

        /// <summary>
        /// 受益人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? singapore_universal_beneficiary_id { get; set; }

        /// <summary>
        /// 付费人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? singapore_universal_payer_id { get; set; }
    }

    /// <summary>
    /// 台湾金融服务（Finserv）
    /// </summary>
    public class TaiwanFinserv
    {
        public TaiwanFinserv()
        {
        }

        public TaiwanFinserv(string? beneficiary_id, string? finserv_funder_id)
        {
            this.taiwan_finserv_beneficiary_id = beneficiary_id;
            this.taiwan_finserv_funder_id = finserv_funder_id;
        }

        /// <summary>
        /// 受益人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? taiwan_finserv_beneficiary_id { get; set; }

        /// <summary>
        /// 付费人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? taiwan_finserv_funder_id { get; set; }
    }

    /// <summary>
    /// 澳大利亚金融服务公司（Finserv）
    /// </summary>
    public class AustraliaFinserv
    {
        public AustraliaFinserv()
        {
        }

        public AustraliaFinserv(string? finserv_beneficiary_id, string? finserv_payer_id)
        {
            this.australia_finserv_beneficiary_id = finserv_beneficiary_id;
            this.australia_finserv_payer_id = finserv_payer_id;
        }

        /// <summary>
        /// 受益人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? australia_finserv_beneficiary_id { get; set; }

        /// <summary>
        /// 付费人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? australia_finserv_payer_id { get; set; }
    }

    /// <summary>
    /// 台湾环球
    /// </summary>
    public class TaiwanUniversal
    {
        public TaiwanUniversal()
        {
        }

        public TaiwanUniversal(string? universal_beneficiary_id,string? universal_payer_id)
        {
            this.taiwan_universal_beneficiary_id = universal_beneficiary_id;
            this.taiwan_universal_payer_id = universal_payer_id;
        }

        /// <summary>
        /// 受益人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? taiwan_universal_beneficiary_id { get; set; }

        /// <summary>
        /// 付费人
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? taiwan_universal_payer_id { get; set; }
    }

    public class AdCreativeDataInfoDto
    {
        /// <summary>
        /// 创意信息
        /// </summary>
        public MetaAdCreativeObjectStorySpecDto? object_story_spec { get; set; }

        /// <summary>
        /// 动态广告创意
        /// </summary>
        public MetaAdAssetFeedSpecDto? asset_feed_spec { get; set; }

        /// <summary>
        /// 创意id
        /// </summary>
        public string id { get; set; } = null!;

        /// <summary>
        /// 广告账户id
        /// </summary>
        public string account_id { get; set; } = null!;

        /// <summary>
        /// 标题
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// 导向类型
        /// </summary>
        public string? call_to_action_type { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string? name { get; set; }

        /// <summary>
        /// 图片hash
        /// </summary>
        public string? image_hash { get; set; }

        /// <summary>
        /// 商店url
        /// </summary>
        public string? object_store_url { get; set; }

        /// <summary>
        /// 缩略图url
        /// </summary>
        public string? thumbnail_url { get; set; }

    }

    public class DegreesOfFreedomSpecDto
    {
        public CreativeFeaturesSpecDto creative_features_spec { get; set; } = null!;

    }

    public class CreativeFeaturesSpecDto
    {
        public CreativeFeaturesSpecDto() { }

        /// <summary>
        /// 
        /// </summary>
        public StandardEnhancementsDto? standard_enhancements { get; set; }

        /// <summary>
        /// 突显重要评论
        /// </summary>
        public StandardEnhancementsDto? inline_comment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public StandardEnhancementsDto? advantage_plus_creative { get; set; }

        /// <summary>
        /// 添加3D动画
        /// </summary>
        public StandardEnhancementsDto? cv_transformation{ get; set; }

        /// <summary>
        /// 添加图片滤镜
        /// </summary>
        public StandardEnhancementsDto? image_enhancement { get; set; }

        /// <summary>
        /// 添加图片模板
        /// </summary>
        public StandardEnhancementsDto? image_templates { get; set; }

        /// <summary>
        /// 调整图像素材 
        /// </summary>
        public StandardEnhancementsDto? image_touchups { get; set; }

        /// <summary>
        /// 扩展图片
        /// </summary>
        public StandardEnhancementsDto? image_uncrop { get; set; }

        /// <summary>
        /// 美化文字
        /// </summary>
        public StandardEnhancementsDto? text_optimizations { get; set; }

        public StandardEnhancementsDto? media_type_automation { get; set; }

        /// <summary>
        /// 定制说明
        /// </summary>
        public StandardEnhancementsDto? description_automation { get; set; }

        /// <summary>
        /// 突显卡片
        /// </summary>
        public StandardEnhancementsDto? media_order { get; set; }

        /// <summary>
        /// 添加目录中的商品
        /// </summary>
        public StandardEnhancementsDto? product_extensions { get; set; }

        /// <summary>
        /// 添加资料图卡
        /// </summary>
        public StandardEnhancementsDto? profile_card { get; set; }

        /// <summary>
        /// 标准美化处理
        /// </summary>
        public void SetStandardEnhancements()
        {
            if(!string.IsNullOrEmpty(this.standard_enhancements?.enroll_status)) return;

            var isOpen = this.inline_comment?.Set() ?? false ||
                (this.cv_transformation?.Set() ?? false) ||
                (this.image_enhancement?.Set() ?? false) ||
                (this.image_templates?.Set() ?? false) ||
                (this.image_touchups?.Set() ?? false) ||
                (this.image_uncrop?.Set() ?? false) ||
                (this.text_optimizations?.Set() ?? false) ||
                (this.media_type_automation?.Set() ?? false) ||
                (this.description_automation?.Set() ?? false) ||
                (this.media_order?.Set() ?? false) ||
                (this.product_extensions?.Set() ?? false) ||
                (this.profile_card?.Set() ?? false);

            //this.standard_enhancements=new StandardEnhancementsDto(isOpen);
        }
    }

    public class StandardEnhancementsDto
    {
        public StandardEnhancementsDto()
        {
            this.enroll_status = "OPT_IN";
        }

        public StandardEnhancementsDto(bool isOpt)
        {
            this.enroll_status = isOpt ? "OPT_IN" : "OPT_OUT";
        }

        /// <summary>
        /// 媒体回显bool值处理
        /// </summary>
        /// <returns></returns>
        public bool Set()
        {
            return this.enroll_status == "OPT_IN";
        }

        public string enroll_status { get; set; }
    }
}
