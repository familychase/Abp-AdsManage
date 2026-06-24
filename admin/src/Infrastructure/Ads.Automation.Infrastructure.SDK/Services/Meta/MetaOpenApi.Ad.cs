
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 分页获取广告列表.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>        
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="effective_status">广告状态</param>
        /// <param name="adNos">广告列表</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Ad>> GetAdsAsync(AccessIdentity identity, string accountNo, int limit, string queryFields, List<string> effective_status = null!, List<string> adNos = null!)
        {
            var request = identity.Request($"/{accountNo}/ads")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (effective_status != null)
            {
                request.AddQueryParameter(nameof(effective_status), effective_status.ToJsonIgnoreNullValue());
            }

            if (adNos != null && adNos.Any())
            {
                request.AddQueryParameter("filtering", $"[{{field:\"id\",operator:\"IN\",value:[\"{string.Join("\",\"", adNos)}\"]}}]");
            }

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Ad>>(request) ?? new MetaPagedDto<MetaDomain.Ad>();

            //return await Client(UrlConst.MetaGraphApi).MetaConcurrencyGetAsync<MetaDomain.Ad>(request) ??
            //       new MetaPagedDto<MetaDomain.Ad>();
        }

        /// <summary>
        /// 分页获取广告列表.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>        
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Ad>> GetAdsAsync(AccessIdentity identity, string accountNo, int limit, string queryFields,string effectiveStatus,long sinceTime = 0)
        {
            var request = identity.Request($"/{accountNo}/ads")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .AddQueryParameter("effective_status", effectiveStatus)
                .UseVersion(ApiDefaultVersion);

            if (string.IsNullOrWhiteSpace(effectiveStatus) == false)
            {
                request.AddQueryParameter("effective_status", effectiveStatus);
            }

            if (sinceTime > 0)
            {
                request.AddQueryParameter("updated_since", sinceTime);
            }


            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Ad>>(request) ?? new MetaPagedDto<MetaDomain.Ad>();
        }




        /// <summary>
        /// 获取单个广告.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adNo"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static async Task<MetaDomain.Ad> GetAdAsync(AccessIdentity identity, string adNo, string queryFields)
        {
            var request = identity.Request($"/{adNo}")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.Ad>(request))!;
        }

        /// <summary>
        /// 获取广告系列下的所有广告.
        /// </summary>
        public static async Task<MetaPagedDto<MetaDomain.Ad>> GetAdsByCampaignAsync(
            AccessIdentity identity, string campaignNo, int limit, string queryFields)
        {
            var request = identity.Request($"/{campaignNo}/ads")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi)
                .ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Ad>>(request)
                ?? new MetaPagedDto<MetaDomain.Ad>();
        }


        /// <summary>
        /// 获取账户的违规广告列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        public static async Task<JsonObject> GetViolationAdsAsync(AccessIdentity identity, string accountNo, int limit, string queryFields)
        {
            var request = identity.Request($"/{accountNo}/ads")
                 .AddQueryParameter(MetaConst.Fields, queryFields)
                 .AddQueryParameter(MetaConst.Limit, limit)
                 .AddQueryParameter("effective_status", new List<string>() { "DISAPPROVED" }.ToJsonIgnoreNullValue())
                 .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<JsonObject>(request))!;
        }


        /// <summary>
        /// 获取广告违规详情
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adNo">广告No</param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static async Task<JsonObject> GetAdViolationDetailAsync(AccessIdentity identity, string adNo, string queryFields)
        {
            var request = identity.Request($"/{adNo}")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<JsonObject>(request))!;
        }


        /// <summary>
        /// 创建广告
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId">广告账户ID</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> AdAddAsync(AccessIdentity identity, string accountId, MetaDomain.Ad parameter)
        {
            var request = identity.Request($"/{accountId}/ads")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return  (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }

        /// <summary>
        /// 更新广告 - 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adId">广告id</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AdUpdateAsync(AccessIdentity identity, string adId, MetaDomain.Ad parameter)
        {
            var request = identity.Request($"/{adId}")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.BoolResult>(request))!;
        }
    }
}
