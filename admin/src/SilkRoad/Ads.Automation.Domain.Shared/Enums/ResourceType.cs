namespace Ads.Automation.Domain.Shared.Enums;

/// <summary>
/// 资源主体类型
/// </summary>
public enum ResourceType : byte
{
    /// <summary>
    /// 无
    /// </summary>
    NONE = 0,
    
    /// <summary>
    /// 授权渠道
    /// </summary>
    CHANNEL = 1,
    
    /// <summary>
    /// 广告账户
    /// </summary>
    AD_ACCOUNT = 2,
}
