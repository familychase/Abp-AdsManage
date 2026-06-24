namespace Ads.Automation.Domain.Shared.Localization;

/// <summary>
/// Fallback validation messages for DataAnnotations attributes.
/// These are compile-time constants; IStringLocalizer handles runtime i18n.
/// </summary>
public static class V
{
    public static string UserCodeRequired => "User code is required";
    public static string PasswordRequired => "Password is required";
    public static string CampaignNoRequired => "Campaign number is required";
    public static string AccountNoRequired => "Account number is required";
}
