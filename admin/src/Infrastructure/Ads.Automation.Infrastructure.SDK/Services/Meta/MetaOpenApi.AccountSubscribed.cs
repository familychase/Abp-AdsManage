
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// AccountSubscribedApps  --账户订阅应用
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取账户订阅应用，是否订阅meta的AppId
        /// </summary>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AccountSubscribedApps>> GetAccountSubscribedAppsAsync(AccessIdentity identity, string accountNo)
        {
            var request = identity.Request($"/{accountNo}/subscribed_apps")
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AccountSubscribedApps>>(request))!;
        }


        /// <summary>
        /// 账户订阅应用
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">媒体广告账户Id</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AccountSubscribedAppsAsync(AccessIdentity identity,string accountNo)
        {
            var request = identity.Request($"/{accountNo}/subscribed_apps")
                .AddQueryParameter("app_id", identity.AppKey)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).PostAsync<MetaOutput.BoolResult>(request))!;
        }

    }
}
