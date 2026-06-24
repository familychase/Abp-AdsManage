
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {

        /// <summary>
        /// 获取账户所有应用
        /// </summary>
        /// <param name="accountId">广告账户ID</param>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AppInfo>?> GetAppsAsync(AccessIdentity identity, string accountId, string fields)
        {
            // ReSharper disable once StringLiteralTypo
            var request = identity.Request($"/{accountId}/advertisable_applications")
                 .AddQueryParameter(MetaConst.Fields, fields)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AppInfo>>(request);
        }


        /// <summary>
        /// 获取应用信息
        /// </summary>
        /// <param name="appId">应用id</param>
        /// <param name="identity"></param>
        /// <param name="fields">默认可返回字段列 fileds= "id,icon_url,name,object_store_urls,supported_platforms"</param>
        /// <returns></returns>
        public static async Task<MetaDomain.AppInfo?> GetAppInfoAsync(AccessIdentity identity, string appId,string fields)
        {
            //获取方法
            var request = identity.Request($"/{appId}")
               .AddQueryParameter(MetaConst.Fields, fields)
               .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AppInfo>(request);
        }
    }
}
