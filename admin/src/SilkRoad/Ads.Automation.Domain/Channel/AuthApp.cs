using Ads.Automation.Domain.Shared.Enums;
using Ads.Automation.Infrastructure.Repository;
using Ads.Automation.Infrastructure.Yitter;

namespace Ads.Automation.Domain.Channel;

/// <summary>
/// 第三方授权应用实体
/// </summary>
public class AuthApp : AggregateRootEntity, IHasCreationTimeEntity
{
    /// <summary>
    /// 平台
    /// </summary>
    public PlatformType Platform { get; private set; }

    /// <summary>
    /// 应用 ID
    /// </summary>
    public string AppId { get; private set; } = string.Empty;

    /// <summary>
    /// AppKey
    /// </summary>
    public string AppKey { get; private set; } = string.Empty;

    /// <summary>
    /// AppSecreat
    /// </summary>
    public string AppSecreat { get; private set; } = string.Empty;

    // ===== 审计字段 =====

    /// <summary>
    /// 创建人
    /// </summary>
    public long CreatorId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; } = DateTime.Now;

    // ===== 构造函数 =====

    private AuthApp() { }

    public static AuthApp Create(
        PlatformType platform,
        string appId,
        string appKey,
        string appSecreat)
    {
        return new AuthApp(
            IdGenerator.GetNextId(),
            platform,
            appId,
            appKey,
            appSecreat);
    }

    internal AuthApp(
        long id,
        PlatformType platform,
        string appId,
        string appKey,
        string appSecreat)
        : base(id)
    {
        Platform = platform;
        AppId = appId;
        AppKey = appKey;
        AppSecreat = appSecreat;
    }
}
