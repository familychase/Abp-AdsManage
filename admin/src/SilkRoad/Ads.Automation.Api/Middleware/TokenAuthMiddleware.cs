using Microsoft.AspNetCore.Authorization;

namespace Ads.Automation.Api.Middleware
{
    /// <summary>
    /// 令牌认证中间件。  
    /// 从请求头中验证 access_token 是否与 Redis 中的值匹配。  
    /// 跳过带有 [AllowAnonymous] 属性的端点和 /swagger 路由。
    /// </summary>
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICacheService _cacheService;
        private readonly IStringLocalizer<AdsAutomationResource> _localizer;

        public TokenAuthMiddleware(
            RequestDelegate next,
            ICacheService cacheService,
            IStringLocalizer<AdsAutomationResource> localizer)
        {
            _next = next;
            _cacheService = cacheService;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context, UserInfoContext userContext)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

            // 跳过 /swagger 路由
            if (path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // 检查端点是否标注了 [AllowAnonymous]
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                await _next(context);
                return;
            }

            // 从请求头中提取 access_token
            if (!context.Request.Headers.TryGetValue("access_token", out var token) || string.IsNullOrEmpty(token))
            {
                await WriteUnauthorizedResponse(context, _localizer["TokenExpired"]);
                return;
            }

            // 将令牌验证到 Redis
            var tokenKey = $"token:{token}";
            var userInfo = await _cacheService.GetAsync<UserInfoDto>(tokenKey);

            if (userInfo == null)
            {
                await WriteUnauthorizedResponse(context, _localizer["TokenExpired"]);
                return;
            }

            // 为下游使用填充 UserInfoContext（作用域）
            userContext.UserId = userInfo.UserId;
            userContext.UserCode = userInfo.UserCode;
            userContext.UserName = userInfo.UserName;
            userContext.UserInfo = userInfo;

            await _next(context);
        }

        private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json; charset=utf-8";
            var result = WebApiResponse<object>.Fail(message, 401);
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(json);
        }
    }
}
