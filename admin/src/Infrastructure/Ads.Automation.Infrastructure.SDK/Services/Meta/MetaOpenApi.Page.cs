
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取账户主页列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.PromotePage>?> GetPromotePagesAsync(AccessIdentity identity, string accountNo, int limit, string fields)
        {
            var request = identity.Request($"/{accountNo}/promote_pages")
                .AddQueryParameter(MetaConst.Fields, fields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.PromotePage>>(request);
        }


        /// <summary>
        /// 获取用户级主页列表
        /// 返回字段 fields = "id,name,category"
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="limit"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.PromotePage>?> GetUserPagesAsync(AccessIdentity identity, int limit, string fields, string after = null)
        {
            var request = identity.Request($"/{identity.MediaIdentity}/accounts")
               .AddQueryParameter(MetaConst.Fields, fields)
               .AddQueryParameter(MetaConst.Limit, limit)
               .UseVersion(ApiDefaultVersion);

            if (!string.IsNullOrEmpty(after))
                request.AddQueryParameter("after", after);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.PromotePage>>(request);
        }


        /// <summary>
        /// 获取主页信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="pageNo">主页编号</param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaDomain.PromotePage?> GetPageInfoAsync(AccessIdentity identity, string pageNo, string fields)
        {
            //获取方法
            var request = identity.Request($"/{pageNo}")
               .AddQueryParameter(MetaConst.Fields, fields)
               .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.PromotePage>(request);
        }


        /// <summary>
        /// 获取主页帖子列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="pageNo"></param>
        /// <param name="limit"></param>
        /// <param name="fields">字段</param>
        /// <param name="minDate">最小时间</param>
        /// <param name="includeInlineCreate"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdsPosts>> GetAdsPostsListAsync(AccessIdentity identity, string pageNo, int limit, string fields, DateTime minDate, bool includeInlineCreate = true)
        {
            long timestamp = (long)(minDate - new DateTime(1970, 1, 1)).TotalSeconds;

            var request = identity.Request($"/{pageNo}/ads_posts")
                .AddQueryParameter(MetaConst.Fields, fields)
                .AddQueryParameter(MetaConst.IncludeInlineCreate, includeInlineCreate)
                .AddQueryParameter("since", timestamp)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdsPosts>>(request) ?? new MetaPagedDto<MetaDomain.AdsPosts>();
        }


        /// <summary>
        /// 获取帖子详情
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="postNo"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaDomain.AdsPosts?> GetAdsPostsDetailAsync(AccessIdentity identity, string postNo, string fields)
        {
            var request = identity.Request($"/{postNo}")
                .AddQueryParameter(MetaConst.Fields, fields)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AdsPosts>(request);
        }
    }
}
