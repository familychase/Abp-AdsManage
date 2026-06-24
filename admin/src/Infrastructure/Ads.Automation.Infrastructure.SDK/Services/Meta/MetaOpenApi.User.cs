/*
 * @author: hujingtian 2022-12-7 11:29:54
 */

namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<MetaDomain.User> GetUserInfo(string accessToken, string fields)
        {
            var request = new RestRequest("/me")
                .AddHeader("Authorization", $"Bearer {accessToken}")
                .UseVersion(ApiDefaultVersion);

            if (!string.IsNullOrEmpty(fields))
            {
                request.AddQueryParameter(MetaConst.Fields, fields);
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.User>(request))!;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public static async Task<MetaDomain.User> GetUserInfo(string userId,string accessToken, string fields)
        {
            //https://graph.facebook.com/USER-ID?fields=id,name,email,picture&access_token=ACCESS-TOKEN
            var request = new RestRequest($"/{userId}")
                .AddHeader("Authorization", $"Bearer {accessToken}")
                .AddQueryParameter("access_token",accessToken)
                .UseVersion(ApiDefaultVersion);

            if (!string.IsNullOrEmpty(fields))
            {
                request.AddQueryParameter(MetaConst.Fields, fields);
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.User>(request))!;
        }

        /// <summary>
        /// 获取用户的BM信息列表
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="limit">限制的个数</param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.BmInfo>> GetBusinessList(string accessToken, int limit, string fields = null)
        {
            var request = new RestRequest("/me/businesses")
                .AddHeader("Authorization", $"Bearer {accessToken}")
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (!string.IsNullOrEmpty(fields))
            {
                request.AddQueryParameter(MetaConst.Fields, fields);
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.BmInfo>>(request))!;
        }

    }
}
