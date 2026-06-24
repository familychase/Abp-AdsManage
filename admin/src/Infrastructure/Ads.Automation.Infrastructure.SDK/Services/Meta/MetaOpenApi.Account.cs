
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class MetaOpenApi
    {
        /// <summary>
        /// 获取单个广告账户 (详情)
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">广告账户</param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static Task<MetaDomain.AdAccount> GetAdAccountAsync(AccessIdentity identity, string accountNo, string queryFields)
        {
            var request = identity.Request($"/{accountNo}")
                .AddQueryParameter(MetaConst.Fields, queryFields).UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AdAccount>(request)!;
        }


        /// <summary>
        /// 分页获取广告账户列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="limit"></param>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        public static Task<MetaPagedDto<MetaDomain.AdAccount>> GetAdAccountsAsync(AccessIdentity identity, int limit, string queryFields)
        {
            var request = identity.Request($"/{identity.MediaIdentity}/adaccounts")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdAccount>>(request)!;
        }


        /// <summary>
        /// 获取用户下的广告账户总数
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static Task<MetaPagingSummaryDto<MetaDomain.AdAccountSimple>> GetAdAccountsCountAsync(AccessIdentity identity)
        {
            var request = identity.Request($"/{identity.MediaIdentity}/adaccounts")
                .AddQueryParameter(MetaConst.Limit, 1)
                .AddQueryParameter(MetaConst.Summary, "total_count")
                .UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagingSummaryDto<MetaDomain.AdAccountSimple>>(request)!;
        }


        /// <summary>
        /// 分页获取BM的广告账户列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="businessNo">BM信息编号</param>
        /// <param name="limit"></param>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        public static Task<MetaPagedDto<MetaDomain.AdAccount>> GetOwnAdAccountsAsync(AccessIdentity identity, string businessNo, int limit, string queryFields)
        {
            var request = identity.Request($"/{businessNo}/owned_ad_accounts")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdAccount>>(request)!;
        }


        /// <summary>
        /// 根据账户ID 获得该账户下活动信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">账户Id</param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="limit">分页条数</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AccountActivity>> GetAccountActivities(AccessIdentity identity, string accountNo,
            string queryFields, int limit)
        {
            var request = identity.Request($"/{accountNo}/activities")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AccountActivity>>(request))!;
        }

        /// <summary>
        /// 广告表现提升建议
        /// https://developers.facebook.com/docs/marketing-api/overview/performance-recommendations/#applying-recommendations
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AccountRecommendations>> GetAccountRecommendations(AccessIdentity identity, string accountNo)
        {
            var request = identity.Request($"/act_{accountNo}/recommendations")
                //.AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, 20)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AccountRecommendations>>(request))!;
        }

        /// <summary>
        /// 广告表现提升建议
        /// https://developers.facebook.com/docs/marketing-api/overview/performance-recommendations/#applying-recommendations
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> UpdateAccountRecommendations(AccessIdentity identity, string accountNo, MetaInput.AccountRecommendations parameter)
        {
            var request = identity.Request($"/act_{accountNo}/recommendations")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.BoolResult>(request))!;
        }

    }

}
