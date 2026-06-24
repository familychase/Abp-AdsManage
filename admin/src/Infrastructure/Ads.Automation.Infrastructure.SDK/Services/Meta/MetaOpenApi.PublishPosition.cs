
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取账户有效的发布版位列表
        /// </summary>
        /// <returns></returns>
        public static async Task<MetaDomain.PublishPosition?> GetPublishPositionAsync(AccessIdentity identity, MetaInput.PublishPositionQuery query)
        {
            var request = identity.Request($"/ad_campaign_placement")
                .AddQueryParameter(nameof(query.account_id), query.account_id)
                .AddQueryParameter(nameof(query.billing_event), query.billing_event)
                .AddQueryParameter(nameof(query.buying_type), query.buying_type)
                .AddQueryParameter(nameof(query.objective), query.objective)
                .AddQueryParameter(nameof(query.optimization_goal), query.optimization_goal)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.PublishPosition>(request);
        }


    }
}
