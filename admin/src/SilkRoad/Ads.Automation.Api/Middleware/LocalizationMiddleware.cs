using Ads.Automation.Domain.Shared.Common;
using Ads.Automation.Domain.Shared.Enums;
using System.Globalization;

namespace Ads.Automation.Api.Middleware
{
    /// <summary>
    /// 本地化中间件：读取请求头 accept-language，设置 CultureInfo 和 LocalizationContext
    /// </summary>
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, LocalizationContext localizationContext)
        {
            var (cultureName, languageType) = ResolveCulture(context);

            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            localizationContext.LanguageType = languageType;

            await _next(context);
        }

        /// <summary>
        /// 从请求头 accept-language 解析语言，返回 (CultureName, GlobalLanguageType)
        /// </summary>
        private static (string CultureName, GlobalLanguageType LanguageType) ResolveCulture(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("accept-language", out var headerValue)
                || string.IsNullOrWhiteSpace(headerValue))
            {
                return ("zh-Hans", GlobalLanguageType.ZH);
            }

            var lang = headerValue.ToString().Trim().ToLowerInvariant();

            return lang switch
            {
                "en" or "en-us" => ("en", GlobalLanguageType.EN),
                _ => ("zh-Hans", GlobalLanguageType.ZH)
            };
        }
    }
}
