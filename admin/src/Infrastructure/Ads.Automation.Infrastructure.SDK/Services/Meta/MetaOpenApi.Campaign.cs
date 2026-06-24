
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 分页获取 Campaign。
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="campaignNos"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdCampaign>> GetCampaignsAsync(AccessIdentity identity, string accountNo, int limit, string queryFields, List<string>? campaignNos = null)
        {
            var request = identity.Request($"/{accountNo}/campaigns")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (campaignNos != null && campaignNos.Any())
            {
                request.AddQueryParameter("filtering", $"[{{field:\"id\",operator:\"IN\",value:[\"{string.Join("\",\"", campaignNos)}\"]}}]");
            }

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdCampaign>>(request) ?? new MetaPagedDto<MetaDomain.AdCampaign>();
        }

        /// <summary>
        /// 分页获取 Campaign。
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="effectiveStatus"></param>
        /// <param name="sinceTime"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdCampaign>> GetCampaignsByConditionAsync(AccessIdentity identity, string accountNo, int limit, string queryFields, string effectiveStatus,long sinceTime = 0)
        {
            var request = identity.Request($"/{accountNo}/campaigns")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);
          

            if (string.IsNullOrWhiteSpace(effectiveStatus) == false)
            {
                request.AddQueryParameter("effective_status", effectiveStatus);
            }

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdCampaign>>(request) ?? new MetaPagedDto<MetaDomain.AdCampaign>();
        }


        /// <summary>
        /// 获取单个 Campaign.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="campaignNo"></param>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        public static async Task<MetaDomain.AdCampaign?> GetCampaignAsync(AccessIdentity identity, string campaignNo, string queryFields)
        {
            var request = identity.Request($"/{campaignNo}")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AdCampaign>(request);
        }


        /// <summary>
        /// 添加广告系列
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId">广告账户id</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> AdCampaignAddAsync(AccessIdentity identity, string accountId, MetaDomain.AdCampaign parameter)
        {
            var request = identity.Request($"/{accountId}/campaigns")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }


        /// <summary>
        /// 更新广告系列
        /// https://developers.facebook.com/docs/marketing-api/reference/ad-campaign-group
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="campaignId">广告系列id</param> 
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AdCampaignUpdateAsync(AccessIdentity identity, string campaignId, MetaInput.AdCampaignsUpdateParameter parameter)
        {
            var request = identity.Request($"/{campaignId}")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                 .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.BoolResult>(request))!;
        }


        /// <summary>
        /// 删除广告系列/广告组/广告编号
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="campaignId">广告系列/广告组/广告编号</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AdManagementDeleteAsync(AccessIdentity identity, string campaignId)
        {
            var request = identity.Request($"/{campaignId}")
                 .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyDeleteAsync<MetaOutput.BoolResult>(request))!;
        }


        #region 获取广告系列，广告组，广告列表

        /// <summary>
        /// 广告管理（广告系列、广告组）账户内复制    第一步获取复制缓存信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="copyDto"></param>MetaDomain.AdObjectIdInfo
        /// <returns></returns>
        public static async Task<MetaDomain.AsyncSessions> AdManagementSessionsGetAsync(AccessIdentity identity, MetaInput.CopyDto copyDto)
        {
            var request = identity.Request("")
                .AddJsonBody(copyDto)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaDomain.AsyncSessions>(request))!;
        }

        /// <summary>
        /// 广告管理（广告系列、广告组）账户内复制    第二步复制缓存信息的结果
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="sessionsId">AsyncSessions!.async_sessions[0].id</param>
        /// <returns></returns>
        public static async Task<MetaDomain.AsyncSessionResult> AdManagementSessionResultGetAsync(AccessIdentity identity, string sessionsId)
        {
            var request = identity.Request($"/{sessionsId}")
                .AddQueryParameter(MetaConst.Fields, "result")
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AsyncSessionResult>(request))!;
        }

        #endregion


        /// <summary>
        /// 获取广告系列的广告组信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="campaignNo"></param>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        public static async Task<JsonObject?> GetAdsetByCampaignAsync(AccessIdentity identity, string campaignNo, List<string> ids, string queryFields)
        {
            var request = identity.Request($"/{campaignNo}")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(nameof(ids), string.Join(",", ids))
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<JsonObject>(request);
        }
    }
}
