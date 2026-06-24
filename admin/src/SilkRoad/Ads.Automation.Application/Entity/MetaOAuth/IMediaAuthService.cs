namespace Ads.Automation.Application.Entity.MetaOAuth;

/// <summary>
/// 媒体授权服务 —— 全局统一的授权身份获取入口。
/// 封装了"渠道查找 + Token 解密"逻辑，所有需要 Meta API 授权的地方统一调用此服务。
/// </summary>
public interface IMediaAuthService
{
    /// <summary>
    /// 通过渠道 ID 直接获取 AccessIdentity
    /// </summary>
    Task<AccessIdentity> GetByChannelIdAsync(long channelId);

    /// <summary>
    /// 根据广告账户编号查找关联渠道，获取 AccessIdentity
    /// </summary>
    Task<AccessIdentity> GetByAccountNoAsync(string accountNo);

    /// <summary>
    /// 获取任意可用渠道的 AccessIdentity（不限制账户）
    /// </summary>
    Task<AccessIdentity> GetFromAnyChannelAsync();
}
