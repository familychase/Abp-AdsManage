namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 使用 OAuth 授权码换取短期访问令牌（Authorization Code Grant）
        /// 对应 Meta OAuth 回调流程第一步：code → short-lived token
        /// </summary>
        /// <param name="code">Meta OAuth 回调返回的授权码</param>
        /// <param name="appId">Meta App ID</param>
        /// <param name="appSecret">Meta App Secret</param>
        /// <param name="redirectUri">OAuth 回调地址，必须与授权请求中的 redirect_uri 完全一致</param>
        /// <returns>短期访问令牌</returns>
        public static async Task<MetaDomain.AccessToken> ExchangeAuthorizationCodeAsync(
            string code, string appId, string appSecret, string redirectUri)
        {
            var request = new RestRequest("/oauth/access_token")
                .AddQueryParameter("client_id", appId)
                .AddQueryParameter("redirect_uri", redirectUri)
                .AddQueryParameter("client_secret", appSecret)
                .AddQueryParameter("code", code)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi)
                .ConcurrencyGetAsync<MetaDomain.AccessToken>(request))!;
        }
    }
}
