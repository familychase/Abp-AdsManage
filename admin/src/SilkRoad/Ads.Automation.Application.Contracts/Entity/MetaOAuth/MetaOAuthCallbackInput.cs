using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Ads.Automation.Application.Contracts.Entity.MetaOAuth
{
    /// <summary>
    /// Meta OAuth 授权回调输入参数
    /// </summary>
    public class MetaOAuthCallbackInput
    {
        /// <summary>
        /// Meta OAuth 回调返回的授权码
        /// </summary>
        [Required]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Meta 应用的 AppId，用于定位 auth_app 配置并作为渠道去重键
        /// </summary>
        [Required]
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// OAuth 回调的 redirect_uri（必须与授权请求一致）
        /// </summary>
        [Required]
        public string RedirectUri { get; set; } = string.Empty;
        
    }
}
