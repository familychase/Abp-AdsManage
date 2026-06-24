using Ads.Automation.Domain.Shared.Enums.Publishing;
using Ads.Automation.Infrastructure.Repository;

namespace Ads.Automation.Domain.Publishing.ValueObjects;

/// <summary>
/// 广告发布模板统计信息
/// </summary>
public sealed class AdsPublishingTemplateStats : ValueObject
{
    /// <summary>
    /// ctor
    /// </summary>
    /// <param name="publishCount">累计发布次数</param>
    /// <param name="publishAdCount">累计发布广告次数</param>
    public AdsPublishingTemplateStats(int publishCount, int publishAdCount)
    {
        PublishCount = publishCount;
        PublishAdCount = publishAdCount;
    }

    /// <summary>
    /// 设置当前模板发布数量
    /// </summary>
    /// <param name="publishAdCount">发布广告数</param>
    public void SetPublishCount(int publishAdCount)
    {
        PublishCount++;
        PublishAdCount += publishAdCount;
    }

    /// <summary>累计发布次数</summary>
    public int PublishCount { get; private set; }

    /// <summary>累计发布广告次数</summary>
    public int PublishAdCount { get; private set; }

    /// <inheritdoc />
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PublishCount;
        yield return PublishAdCount;
    }
}
