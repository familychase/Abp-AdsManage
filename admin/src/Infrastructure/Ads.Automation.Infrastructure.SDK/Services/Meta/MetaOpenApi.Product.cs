
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取BM下的商品目录
        /// 返回字段 fields= "id,name,product_count,vertical"
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="businessNo">Bm的id</param>
        /// <param name="limit"></param>
        /// <param name="fields"></param>
        public static async Task<MetaPagedDto<MetaDomain.ProductCatalogs>?> GetOwnedProductCatalogsAsync(AccessIdentity identity, string businessNo, int limit, string fields)
        {
            var request = identity.Request($"/{businessNo}/owned_product_catalogs")
                .AddQueryParameter(MetaConst.Fields, fields)
                .AddQueryParameter(MetaConst.Limit, limit)
               .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.ProductCatalogs>>(request);
        }


        /// <summary>
        /// 获取BM下的商品系列
        /// 返回字段 fields= "id,name,filter,product_count"
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="catalogNo"></param>
        /// <param name="limit"></param>
        /// <param name="fields"></param>
        public static async Task<MetaPagedDto<MetaDomain.ProductCatalogs>?> GetProductSetsAsync(AccessIdentity identity, string catalogNo, int limit,
            string fields)
        {
            var request = identity.Request($"/{catalogNo}/product_sets")
                .AddQueryParameter(MetaConst.Fields, fields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.ProductCatalogs>>(request);
        }

    }
}
