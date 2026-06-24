using Ads.Automation.Domain.Shared.Enums.Publishing;
using Ads.Automation.Infrastructure.Repository;

namespace Ads.Automation.Domain.Publishing.ValueObjects;

/// <summary>
/// 广告批量发布选项
/// </summary>
public sealed class AdsBatchPublishingOptions : ValueObject
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="publishingType">批量发布类型</param>
    /// <param name="maxPublishCount">每广告组循序发布的最大广告数</param>
    /// <param name="publishAverage">是否尽量平均发布</param>
    public AdsBatchPublishingOptions(AdsBatchPublishingType publishingType, int maxPublishCount, bool publishAverage)
    {
        PublishingType = publishingType;
        MaxPublishCount = maxPublishCount;
        PublishAverage = publishAverage;
    }

    /// <summary>批量发布类型</summary>
    public AdsBatchPublishingType PublishingType { get; private set; }

    /// <summary>每个广告组允许发布的最大广告条数</summary>
    public int MaxPublishCount { get; private set; }

    /// <summary>是否尽量平均发布</summary>
    public bool PublishAverage { get; private set; }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PublishingType;
        yield return MaxPublishCount;
        yield return PublishAverage;
    }
}
