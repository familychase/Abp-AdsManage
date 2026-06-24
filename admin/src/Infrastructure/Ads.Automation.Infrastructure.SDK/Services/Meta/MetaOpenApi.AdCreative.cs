
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public static partial class MetaOpenApi
    {
        /// <summary>
        /// 分页获取获取广告创意列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="limit">分页条数</param>
        /// <param name="accountId">广告账户id</param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static Task<MetaPagedDto<AdCreativeDataInfoDto>> GetAdCreativesAsync(AccessIdentity identity, int limit, string accountId,string queryFields)
        {
            var request = identity.Request($"/{accountId}/adcreatives")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<AdCreativeDataInfoDto>>(request)!;
        }


        /// <summary>
        /// 添加广告创意
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId">广告账户id</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> AdCreativesAddAsync(AccessIdentity identity, string accountId, MetaInput.AdCreativeAddParameter parameter)
        {
            var request = identity.Request($"/{accountId}/adcreatives")
                .AddParameter("application/json", parameter.ToJsonIgnoreNullValue(), ParameterType.RequestBody)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }


        /// <summary>
        /// 更新广告创意
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="creativeId"></param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.BoolResult> AdCreativesUpdateAsync(AccessIdentity identity, string creativeId, MetaInput.AdCreativeUpdateParameter parameter)
        {
            var request = identity.Request($"/{creativeId}")
                .AddJsonBody(parameter)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.BoolResult>(request))!;
        }
    }
}
