// ReSharper disable InconsistentNaming

namespace Ads.Automation.Infrastructure.SDK.Models.Meta.Creative
{
    /// <summary>
    /// Meta - 动态广告创意（Asset Feed Spec）
    /// </summary>
    public class MetaAdAssetFeedSpecDto
    {
        /// <summary>
        /// 图片列表
        /// </summary>
        public List<MetaAdAssetFeedSpecImageDto>? images { get; set; }

        /// <summary>
        /// 视频列表
        /// </summary>
        public List<MetaAdAssetFeedSpecVideoDto>? videos { get; set; }

        #region 文案部分

        /// <summary>
        /// 动态广告标题
        /// </summary>
        public List<MetaAdAssetFeedSpecTextDto>? titles { get; set; }

        /// <summary>
        /// 动态广告正文 - 最多5个
        /// </summary>
        public List<MetaAdAssetFeedSpecTextDto>? bodies { get; set; }

        /// <summary>
        /// 动态广告描述 - 最多5个
        /// </summary>
        public List<MetaAdAssetFeedSpecTextDto>? descriptions { get; set; }

        #endregion

        /// <summary>
        /// 行为导向列表
        /// </summary>
        public List<string>? call_to_action_types { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public List<MetaAdAssetFeedSpecLinkUrlDto>? link_urls { get; set; }

        /// <summary>
        /// 广告格式
        /// </summary>
        public List<string>? ad_formats { get; set; }

        /// <summary>
        /// 自定义商品页面
        /// </summary>
        public string? app_product_page_id { get; set; }

        /// <summary>
        /// 优化类型
        /// </summary>
        public string? optimization_type { get; set; }

        #region 多语言广告字段

        /// <summary>
        /// 资产自定义规则（多语言广告核心）
        /// 用于将语言标签与具体素材关联
        /// </summary>
        public List<AssetCustomizationRuleDto>? asset_customization_rules { get; set; }

        /// <summary>
        /// 创意级广告标签
        /// </summary>
        public List<AdLabelDto>? adlabels { get; set; }

        #endregion

        #region Inner DTOs

        /// <summary>
        /// 动态创意 - 文本
        /// </summary>
        public class MetaAdAssetFeedSpecTextDto
        {
            /// <summary>
            /// 文本信息
            /// </summary>
            public string text { get; set; } = null!;

            /// <summary>
            /// 文案级广告标签列表
            /// </summary>
            public List<AdLabelDto>? adlabels { get; set; }
        }

        /// <summary>
        /// 动态创意 - 链接地址
        /// </summary>
        public class MetaAdAssetFeedSpecLinkUrlDto
        {
            /// <summary>
            /// 地址，当前为应用地址
            /// </summary>
            public string website_url { get; set; } = null!;

            /// <summary>
            /// 显示地址
            /// </summary>
            public string display_url { get; set; } = null!;

            /// <summary>
            /// 深度链接
            /// </summary>
            public string deeplink_url { get; set; } = null!;

            /// <summary>
            /// 链接级广告标签列表
            /// </summary>
            public List<AdLabelDto>? adlabels { get; set; }
        }

        /// <summary>
        /// 动态创意 - 图片信息
        /// </summary>
        public class MetaAdAssetFeedSpecImageDto
        {
            /// <summary>
            /// 图片 hash 值
            /// </summary>
            public string hash { get; set; } = null!;

            /// <summary>
            /// 图片裁剪规格 (e.g. {"90x160": [[163,0],[685,928]]})
            /// key 为裁剪尺寸如 "90x160"，value 为坐标对 [[x1,y1],[x2,y2]]
            /// </summary>
            public Dictionary<string, List<List<int>>>? image_crops { get; set; }

            /// <summary>
            /// 图片级广告标签列表
            /// </summary>
            public List<AdLabelDto>? adlabels { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                var other = (MetaAdAssetFeedSpecImageDto)obj;
                return this.hash == other.hash;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + hash.GetHashCode();
                    return hash;
                }
            }
        }

        /// <summary>
        /// 动态创意 - 视频信息
        /// </summary>
        public class MetaAdAssetFeedSpecVideoDto
        {
            /// <summary>
            /// 视频 Id
            /// </summary>
            public string video_id { get; set; } = null!;

            /// <summary>
            /// 缩略图地址
            /// </summary>
            public string thumbnail_url { get; set; } = null!;

            /// <summary>
            /// 视频级广告标签列表
            /// </summary>
            public List<AdLabelDto>? adlabels { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                    return false;

                var other = (MetaAdAssetFeedSpecVideoDto)obj;
                return this.video_id == other.video_id && this.thumbnail_url == other.thumbnail_url;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + video_id.GetHashCode();
                    hash = hash * 23 + (thumbnail_url != null ? thumbnail_url.GetHashCode() : 0);
                    return hash;
                }
            }
        }

        #endregion
    }

    #region 多语言广告辅助 DTO

    /// <summary>
    /// 广告标签（AdLabel）
    /// </summary>
    public class AdLabelDto
    {
        /// <summary>
        /// 标签 ID（跨账户复制时不传，由 Meta 自动生成）
        /// </summary>
        public string? id { get; set; } 

        /// <summary>
        /// 标签名称（必填）
        /// </summary>
        public string? name { get; set; }
    }

    /// <summary>
    /// 资产自定义规则（Asset Customization Rule）
    /// 用于多语言广告的语言-素材映射规则
    /// </summary>
    public class AssetCustomizationRuleDto
    {
        /// <summary>
        /// 自定义规格（如 locales 列表）
        /// </summary>
        public AssetCustomizationSpecDto? customization_spec { get; set; }

        /// <summary>
        /// 正文关联的标签名称
        /// </summary>
        public AssetLabelReferenceDto? body_label { get; set; }

        /// <summary>
        /// 标题关联的标签名称
        /// </summary>
        public AssetLabelReferenceDto? title_label { get; set; }

        /// <summary>
        /// 描述关联的标签名称
        /// </summary>
        public AssetLabelReferenceDto? description_label { get; set; }

        /// <summary>
        /// 链接关联的标签名称
        /// </summary>
        public AssetLabelReferenceDto? link_url_label { get; set; }

        /// <summary>
        /// 图片关联的标签名称
        /// </summary>
        public AssetLabelReferenceDto? image_label { get; set; }
        
        /// <summary>
        /// 视频关联的标签名称
        /// </summary>
        public AssetLabelReferenceDto? video_label { get; set; }

        /// <summary>
        /// 是否为默认规则
        /// </summary>
        public bool? is_default { get; set; }
    }

    /// <summary>
    /// 标签引用（只传 name，不传 id）
    /// </summary>
    public class AssetLabelReferenceDto
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string? name { get; set; } 
    }

    /// <summary>
    /// 自定义规格
    /// </summary>
    public class AssetCustomizationSpecDto
    {
        /// <summary>
        /// 语言区域 ID 列表（如 17=俄语）
        /// </summary>
        public List<int> locales { get; set; } = new();

        public int? age_max { get; set; }
        public int? age_min { get; set; }
        public List<string>? publisher_platforms { get; set; }
        public List<string>? facebook_positions { get; set; }
        public List<string>? instagram_positions { get; set; }
        public List<string>? messenger_positions { get; set; }
        public List<string>? audience_network_positions { get; set; }
    }

    #endregion
}
