using Ads.Automation.Domain.Shared.Enums.Publishing;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.Publishing.Template;

/// <summary>
/// Meta 平台的广告创意视图模型
/// </summary>
public class MetaAdViewModel
{
    /// <summary>命名规则（支持占位符）</summary>
    public string NameRule { get; set; } = string.Empty;

    /// <summary>
    /// 行动号召类型列表（动态广告可为多个）
    /// 字典枚举：MetaCallActionType
    /// </summary>
    public List<string> CallActionTypes { get; set; } = null!;

    /// <summary>Facebook 主页 ID</summary>
    public string PageNo { get; set; } = null!;

    /// <summary>应用深度链接</summary>
    public string? AppDeepLink { get; set; }

    /// <summary>使用文案中的深度链接</summary>
    public bool? UseLetterDeepLink { get; set; }

    /// <summary>
    /// 网站地址（落地页 URL）
    /// WebUrlNameRule，包含网站地址的命名规则
    /// </summary>
    public string? WebUrl { get; set; }

    /// <summary>显示地址</summary>
    public string DisplayUrl { get; set; } = null!;

    /// <summary>落地页追踪像素编号（仅投放应用时可填）</summary>
    public string TrackPixelNo { get; set; } = null!;

    /// <summary>网域追踪地址（仅投放落地页时可填）</summary>
    public string TrackPixelUrl { get; set; } = null!;

    /// <summary>是否使用公共主页作为广告发布身份</summary>
    public bool? UsePageIdentity { get; set; }

    /// <summary>是否开启进阶赋能素材</summary>
    public bool IsOpenAdvancedMaterial { get; set; }

    /// <summary>是否为多广告组广告</summary>
    public bool MultiAds { get; set; }

    ///// <summary>单图片进阶赋能素材选项</summary>
    //public AdvancedMaterialTypeViewModel? ImageOption { get; set; }

    ///// <summary>单视频进阶赋能素材选项</summary>
    //public AdvancedMaterialTypeViewModel? VideoOption { get; set; }

    ///// <summary>轮播进阶赋能素材选项</summary>
    //public AdvancedMaterialTypeViewModel? CarouselOption { get; set; }

    /// <summary>自定义商品页面 ID</summary>
    public string? AppProductPageId { get; set; }

    /// <summary>是否开启再营销</summary>
    public bool OpenRemarketing { get; set; }

    /// <summary>网站地址（原始地址，供后端调用）</summary>
    public string? OriginWebUrl { get; set; }
}

/// <summary>
/// 进阶赋能素材类型
/// </summary>
public class AdvancedMaterialTypeViewModel
{
    /// <summary>添加目录中的商品</summary>
    public bool ProductExtensions { get; set; }

    /// <summary>调整图像素材</summary>
    public bool ImageTouchups { get; set; }

    /// <summary>美化文字</summary>
    public bool TextOptimizations { get; set; }

    /// <summary>突显重要评论</summary>
    public bool InlineComment { get; set; }

    /// <summary>添加图片模板</summary>
    public bool ImageTemplates { get; set; }

    /// <summary>扩展图片</summary>
    public bool ImageUncrop { get; set; }

    /// <summary>添加 3D 动画</summary>
    public bool CvTransformation { get; set; }

    /// <summary>添加图片滤镜</summary>
    public bool ImageEnhancement { get; set; }

    /// <summary>添加资料图卡</summary>
    public bool ProfileCard { get; set; }

    /// <summary>定制说明</summary>
    public bool DescriptionAutomation { get; set; }

    /// <summary>突显卡片</summary>
    public bool MediaOrder { get; set; }
}
