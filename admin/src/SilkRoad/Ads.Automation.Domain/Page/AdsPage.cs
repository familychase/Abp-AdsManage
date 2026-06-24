using Ads.Automation.Domain.Shared.Enums;

namespace Ads.Automation.Domain.Page;

/// <summary>
/// Facebook 公共主页实体
/// </summary>
public class AdsPage : AggregateRootEntity
{
    /// <summary>
    /// Meta 主页编号
    /// </summary>
    public string PageNo { get; private set; } = string.Empty;

    /// <summary>
    /// 主页名称
    /// </summary>
    public string PageName { get; private set; } = string.Empty;

    /// <summary>
    /// 主页分类
    /// </summary>
    public string? Category { get; private set; }

    /// <summary>
    /// 关联的广告账户号
    /// </summary>
    public string AccountNo { get; private set; } = string.Empty;

    /// <summary>
    /// 媒体平台
    /// </summary>
    public PlatformType Platform { get; private set; }

    /// <summary>
    /// 媒体端资产对接Id
    /// </summary>
    public string? InstagramUserId { get; private set; }

    /// <summary>
    /// 最后同步时间
    /// </summary>
    public DateTime LastSyncTime { get; private set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    // ===== 构造函数 =====

    private AdsPage() { }

    public static AdsPage Create(
        string pageNo,
        string pageName,
        string? category,
        string accountNo,
        PlatformType platform)
    {
        return new AdsPage(
            IdGenerator.GetNextId(),
            pageNo,
            pageName,
            category,
            accountNo,
            platform,
            DateTime.Now,
            DateTime.Now);
    }

    internal AdsPage(
        long id,
        string pageNo,
        string pageName,
        string? category,
        string accountNo,
        PlatformType platform,
        DateTime lastSyncTime,
        DateTime creationTime)
        : base(id)
    {
        PageNo = pageNo;
        PageName = pageName;
        Category = category;
        AccountNo = accountNo;
        Platform = platform;
        InstagramUserId = string.Empty;
        LastSyncTime = lastSyncTime;
        CreationTime = creationTime;
    }

    // ===== 更新方法 =====

    public void SetPageName(string name) => PageName = name;
    public void SetCategory(string? category) => Category = category;
    public void SetLastSyncTime(DateTime time) => LastSyncTime = time;
}
