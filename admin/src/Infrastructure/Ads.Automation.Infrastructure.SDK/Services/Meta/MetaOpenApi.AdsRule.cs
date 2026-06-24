
/*
 * @author: alvin 2022-10-31 10:37
 */

namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 创建广告规则
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Task<MetaOutput.IdResult> CreateAdsRuleAsync(AccessIdentity identity, string accountId, MetaInput.CreateAdsRule rule)
        {
            // ReSharper disable once StringLiteralTypo
            var request = identity.Request($"/{accountId}/adrules_library")
                .AddJsonBody(rule)
                .UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).PostAsync<MetaOutput.IdResult>(request)!;
        }

        /// <summary>
        /// 获取广告账户规则列表.
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Task<MetaPagedDto<MetaOutput.AdsRule>> GetAdsRuleListAsync(AccessIdentity identity, string accountId, string queryFields)
        {
            // ReSharper disable once StringLiteralTypo
            var request = identity.Request($"/{accountId}/adrules_library")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .UseVersion(ApiDefaultVersion);

            return Client(UrlConst.MetaGraphApi).GetAsync<MetaPagedDto<MetaOutput.AdsRule>>(request)!;
        }

        /// <summary>
        /// 删除广告规则.
        /// <see href="https://developers.facebook.com/docs/marketing-api/ad-rules/guides/api-calls">meta apis docs </see>
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="adRuleId"></param>
        /// <returns></returns> 
        public static async Task<MetaOutput.BoolResult> RemoveAdsRuleAsync(AccessIdentity identity, string adRuleId)
        {
            var request = identity.Request($"/{adRuleId}")
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyDeleteAsync<MetaOutput.BoolResult>(request))!;
        }

    }
}
