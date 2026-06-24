
/*
 * @author: alvin 2022-10-24 10:58
 */

namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 分页获取广告组.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">广告账户编号或者广告系列编号</param>
        /// <param name="limit"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="adsetNos">广告组编号列表，筛选</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdSet>> GetAdSetsAsync(AccessIdentity identity, string accountNo, int limit, string queryFields, List<string> adsetNos = null!)
        {
            var request = identity.Request($"/{accountNo}/adsets")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (adsetNos != null && adsetNos.Any())
            {
                request.AddQueryParameter("filtering", $"[{{field:\"id\",operator:\"IN\",value:[\"{string.Join("\",\"", adsetNos)}\"]}}]");
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdSet>>(request))!;
        }


        /// <summary>
        /// 分页获取广告组.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">广告账户编号或者广告系列编号</param>
        /// <param name="limit"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="filter">enum {GREATER_THAN, LESS_THAN, EQUAL, NOT_EQUAL, IN_RANGE, NOT_IN_RANGE, IN, NOT_IN, CONTAIN, NOT_CONTAIN, ANY, ALL, NONE}</param>
        /// <param name="effectiveStatus"> 有效状态 ACTIVE活跃, PAUSED暂停, DELETED已删除, PENDING_REVIEW待审核, DISAPPROVED已拒绝, PREAPPROVED 预审核（刚发布）, PENDING_BILLING_INFO 待结算, CAMPAIGN_PAUSED 暂停, ARCHIVED 归档, ADSET_PAUSED 广告组暂停, IN_PROCESS 处理中, WITH_ISSUES 有问题 </param>
        /// <param name="sinceTime"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdSet>> GetAdSetsByConditionAsync(AccessIdentity identity, string accountNo, int limit, string queryFields, string effectiveStatus,long sinceTime = 0)
        {
            var request = identity.Request($"/{accountNo}/adsets")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            //$"[{{field:\"id\",operator:\"IN\",value:[\"{string.Join("\",\"", adsetNos)}\"]}}]"

            if (string.IsNullOrWhiteSpace(effectiveStatus) == false)
            {
                request.AddQueryParameter("effective_status", effectiveStatus);
            }
           
            if (sinceTime > 0)
            {
                request.AddQueryParameter("updated_since", sinceTime);
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdSet>>(request))!;
        }


        /// <summary>
        /// 获取指定广告系列下所有广告组列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="campaignNo">广告系列Id</param>
        /// <param name="limit"></param>
        /// <param name="queryFields"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdSet>> GetAdSetsByCampaignAsync(AccessIdentity identity, string campaignNo, int limit, string queryFields)
        {
            var request = identity.Request($"/{campaignNo}/adsets")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdSet>>(request))!;
        }


        /// <summary>
        /// 获取单个广告组
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adSetNo"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static async Task<MetaDomain.AdSet> GetAdSetAsync(AccessIdentity identity, string adSetNo, string queryFields)
        {
            var request = identity.Request($"/{adSetNo}")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AdSet>(request))!;
        }


        /// <summary>
        /// 添加广告组
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId">广告账户id</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> AdSetAddAsync(AccessIdentity identity, string accountId, MetaDomain.AdSet parameter)
        {
            var request = identity.Request($"/{accountId}/adsets")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }


        /// <summary>
        /// 更新广告组
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adSetId">广告组id</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AdSetUpdateAsync(AccessIdentity identity, string adSetId, MetaInput.AdSetUpdateParameter parameter)
        {
            var request = identity.Request($"/{adSetId}")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                 .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.BoolResult>(request))!;
        }

        /// <summary>
        /// 更新广告组
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adSetId">广告组id</param>
        /// <param name="parameter">参数</param>
        /// <param name="ifUpdateTime">是否更新结束时间</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AdSetUpdateAsync(AccessIdentity identity, string adSetId, MetaDomain.AdSet parameter, bool ifUpdateTime = false)
        {
            var paraJson = parameter.ToJsonIgnoreNullValue();
            if (ifUpdateTime == true)
            {
                paraJson = paraJson.Substring(0, paraJson.LastIndexOf("}"));

                paraJson += ",\"end_time\":0}";
            }

            //todo:广告编辑使用此方法  后期需要吧预算 竞价金额合成为此方法  - hjt 2024-5-22 09:09:03
            var request = identity.Request($"/{adSetId}")
                .AddParameter("application/json", paraJson, ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.BoolResult>(request))!;
        }


        /// <summary>
        /// 获取广告组受众覆盖率
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo">广告账户</param>
        /// <param name="targeting_spec">受众参数</param>
        /// <returns></returns>
        public static async Task<MetaDomain.AdSetAudienceReachEstimate> ReachEstimateAsync(AccessIdentity identity, string accountNo, MetaDomain.Targeting targeting_spec)
        {
            var request = identity.Request($"{accountNo}/reachestimate")
                .AddQueryParameter(nameof(targeting_spec), targeting_spec.ToJsonIgnoreNullValue())
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AdSetAudienceReachEstimate>(request))!;
        }
    }
}
