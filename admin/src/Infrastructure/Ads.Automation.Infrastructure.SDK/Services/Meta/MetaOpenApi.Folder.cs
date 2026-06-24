
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 获取BM的第一级文件夹
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        public static async Task<MetaPagedDto<MetaDomain.BmFolder>?> GetFirstFolderAsync(AccessIdentity identity, MetaInput.MaterialQuery query)
        {
            var request = identity.Request($"/{query.businessNo}/creative_folders")
                .AddQueryParameter(nameof(query.fields), query.fields)
                .AddQueryParameter(MetaConst.Limit, query.limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.BmFolder>>(request);
        }

        /// <summary>
        /// 获取文件夹的下级文件夹信息
        /// fields = "id,name";
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        public static async Task<MetaPagedDto<MetaDomain.BmFolder>?> GetBmNextFolderAsync(AccessIdentity identity, MetaInput.MaterialQuery query)
        {
            var request = identity.Request($"/{query.creative_folder_id}/subfolders")
                .AddQueryParameter(nameof(query.fields), query.fields)
                .AddQueryParameter(nameof(query.limit), query.limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.BmFolder>>(request);
        }


        /// <summary>
        /// 获取文件夹的素材信息(BM获取)
        /// 返回字段列  fields= "creation_time,duration,hash,height,id,name,thumbnail,type,url,video_id,width"
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.FileInfo>?> GetFolderMaterialAsync(AccessIdentity identity, MetaInput.MaterialQuery query)
        {
            var request = identity.Request($"/{query.businessNo}/creatives")
                .AddQueryParameter(nameof(query.fields), query.fields)
                .AddQueryParameter(nameof(query.creative_folder_id), query.creative_folder_id)
                .AddQueryParameter(nameof(query.limit), query.limit)
                .UseVersion(ApiDefaultVersion);

            if (!string.IsNullOrEmpty(query.before))
            {
                request.AddQueryParameter(nameof(query.before), query.before);
            }

            if (!string.IsNullOrEmpty(query.after))
            {
                request.AddQueryParameter(nameof(query.after), query.after);
            }

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.FileInfo>>(request);
        }


        /// <summary>
        ///  获取文件夹的素材信息(素材编号获取)
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<MetaDomain.BmImagesInfo?> GetMaterialImagesDetailsAsync(AccessIdentity identity, MetaInput.MaterialQuery query)
        {
            var request = identity.Request($"/{query.materialNo}")
                .AddQueryParameter(nameof(query.fields), query.fields)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.BmImagesInfo>(request);
        }


        /// <summary>
        /// 删除文件夹里素材图片或者视频
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="videoIdOrImageId">BM素材文件夹图片id或者视频id</param>
        public static async Task<MetaOutput.BoolResult> DeleteFolderMaterialAsync(AccessIdentity identity, string videoIdOrImageId)
        {
            var request = identity.Request($"/{videoIdOrImageId}")
             .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyDeleteAsync<MetaOutput.BoolResult>(request))!;

        }

        /// <summary>
        /// 向文件夹里添加图片
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="businessNo">Bm的id</param>
        /// <param name="input"></param>
        /// <exception cref="Exception"></exception>
        public static async Task<JsonObject> FolderImageAddAsync(AccessIdentity identity, string businessNo, MetaInput.FolderMaterial input)
        {
            var request = identity.Request($"/{businessNo}/images")
                .AddJsonBody(input.ToJsonIgnoreNullValue())
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<JsonObject>(request))!;
        }

        /// <summary>
        /// 向BM素材文件夹里上传视频
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="businessNo">Bm的id</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> FolderVideoAddAsync(AccessIdentity identity, string businessNo, MetaInput.FolderMaterial input)
        {
            var request = identity.Request($"/{businessNo}/videos")
               .AddJsonBody(input.ToJsonIgnoreNullValue())
               .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }


        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="folderId">文件夹id</param>
        public static async Task<MetaOutput.BoolResult> DeleteFolderAsync(AccessIdentity identity, string folderId)
        {
            var request = identity.Request($"/{folderId}")
              .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyDeleteAsync<MetaOutput.BoolResult>(request))!;
        }

        /// <summary>
        /// 添加BM素材文件夹
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="businessNo">BM编号</param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> AddFolderAsync(AccessIdentity identity, string businessNo, MetaInput.FolderParameter input)
        {
            var request = identity.Request($"/{businessNo}/creative_folders")
                .AddJsonBody(input.ToJsonIgnoreNullValue())
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }

        /// <summary>
        /// 获取BM以及文件夹信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="query"></param>
        public static async Task<MetaDomain.BussinessAndFolder?> GetBusinessesAndFolderAsync(AccessIdentity identity, MetaInput.MaterialQuery query)
        {
            var request = identity.Request($"/me")
                .AddQueryParameter(nameof(query.fields), query.fields)
                .AddQueryParameter(MetaConst.Limit, query.limit)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.BussinessAndFolder>(request);
        }

    }
}
