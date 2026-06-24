
namespace Ads.Automation.Infrastructure.Services.Meta
{
    /// <summary>
    /// 
    /// </summary>
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取广告账户像素列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="limit"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.Pixel>> GetAdPixelsAsync(AccessIdentity identity, string accountNo, int limit, string fields, string? after = null)
        {
            var request = identity.Request($"/{accountNo}/adspixels")
                .AddQueryParameter(MetaConst.Fields, fields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (!string.IsNullOrEmpty(after))
                request.AddQueryParameter("after", after);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.Pixel>>(request) ?? new MetaPagedDto<MetaDomain.Pixel>();
        }

        /// <summary>
        /// 获取广告账户像素自定义事件列表
        /// https://developers.facebook.com/docs/marketing-api/reference/custom-conversion/
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountNo"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.PixelCustomConversions>> GetAdPixelsCustomConversionsAsync(AccessIdentity identity, string accountNo, string fields)
        {
            var request = identity.Request($"/{accountNo}/customconversions")
                .AddQueryParameter(MetaConst.Fields, fields)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.PixelCustomConversions>>(request) ?? new MetaPagedDto<MetaDomain.PixelCustomConversions>();
        }
    }
}
