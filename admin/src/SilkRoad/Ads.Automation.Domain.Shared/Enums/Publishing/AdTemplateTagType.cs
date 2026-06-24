namespace Ads.Automation.Domain.Shared.Enums.Publishing;

/// <summary>
/// 广告模板标签类型（用于筛选和分类模板）
/// </summary>
public enum AdTemplateTagType
{
    /// <summary>定向 - 设置了细分定位（兴趣与行为）</summary>
    DIRECTED,

    /// <summary>通投 - 未设置细分定位</summary>
    GENERAL_INVESTMENT,

    /// <summary>转化量最大化（AEO）</summary>
    AEO,

    /// <summary>转化价值最大化（VO）</summary>
    VO,

    /// <summary>其他转化目标</summary>
    OTHER,

    /// <summary>女性</summary>
    WOMEN,

    /// <summary>男性</summary>
    MEN,

    /// <summary>所有性别</summary>
    ALL,

    /// <summary>Google 日预算</summary>
    GOOGLE_DAILY_BUDGET,
}
