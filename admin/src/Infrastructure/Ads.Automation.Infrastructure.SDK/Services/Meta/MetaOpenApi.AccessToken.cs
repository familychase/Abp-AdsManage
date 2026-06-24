
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取用户登录持久性token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="appKey"></param>
        /// <param name="appSecret"></param>
        /// <returns></returns>
        public static async Task<MetaDomain.AccessToken> GetAccessTokenAsync(string accessToken, string appKey, string appSecret)
        {
            var request = new RestRequest("/oauth/access_token")
                .AddQueryParameter("client_id", appKey)
                .AddQueryParameter("client_secret", appSecret)
                .AddQueryParameter("grant_type", "fb_exchange_token")
                .AddQueryParameter("fb_exchange_token", accessToken)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AccessToken>(request))!;
        }

    }
}
