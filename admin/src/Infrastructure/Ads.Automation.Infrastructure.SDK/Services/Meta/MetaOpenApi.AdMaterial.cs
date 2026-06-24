
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public static partial class MetaOpenApi
    {
        /// <summary>
        /// 分页获取广告图片/根据素材编号获取图片
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="limit"></param>
        /// <param name="accountId"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        ///  <param name="imageNos">图片素材编号</param>
        /// <returns></returns>
        public static Task<MetaPagedDto<MetaDomain.AdImage>> GetAdImagesAsync(AccessIdentity identity, int limit, string accountId, string queryFields, List<string>? imageNos = null)
        {
            var request = identity.Request($"/{accountId}/adimages")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (imageNos is { Count: > 0 })
            {
                request.AddQueryParameter("hashes", imageNos.ToJsonIgnoreNullValue());
            }

            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdImage>>(request)!;
        }


        /// <summary>
        /// 上传图片，只传图片Base64格式
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId"></param>
        /// <param name="imgBase64Code"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<JsonObject> AdImageAddAsync(AccessIdentity identity, string accountId, string imgBase64Code, string name)
        {
            var formData = new Dictionary<string, string>()
            {
                { "bytes", imgBase64Code },
                { "name", name }
            };

            var request = identity.Request($"/{accountId}/adimages")
                .AddBody(formData)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<JsonObject>(request))!;
        }


        /// <summary>
        /// 从其它账户中复制图片
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId"></param>
        /// <param name="sourceAccountId">图片来源账户媒体id</param>
        /// <param name="imageHash">媒体图片hash值</param>
        /// <returns></returns>
        public static async Task<MetaOutput.AdImageCopyResult> AdImageCopyAsync(AccessIdentity identity, string accountId, string sourceAccountId, string imageHash)
        {
            var request = identity
                .Request($"/{accountId}/adimages?copy_from={{\"source_account_id\":\"{sourceAccountId}\",\"hash\":\"{imageHash}\"}}")
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.AdImageCopyResult>(request))!;
        }


        /// <summary>
        /// 分页获取账户广告视频
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId"></param>
        /// <param name="limit"></param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <param name="videoNos">视频编号列表</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.AdVideo>> GetAdVideosAsync(AccessIdentity identity, string accountId, int limit, string queryFields, List<string> videoNos = null!)
        {
            var request = identity.Request($"/{accountId}/advideos")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .AddQueryParameter(MetaConst.Limit, limit)
                .UseVersion(ApiDefaultVersion);

            if (videoNos != null && videoNos.Any())
            {
                request.AddQueryParameter("filtering", $"[{{field:\"id\",operator:\"IN\",value:[\"{string.Join("\",\"", videoNos)}\"]}}]");
            }

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.AdVideo>>(request))!;
        }


        /// <summary>
        /// 获取视频详细信息
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="videoId">视频ID</param>
        /// <param name="queryFields">查询需要返回的字段</param>
        /// <returns></returns>
        public static async Task<MetaDomain.AdVideo?> GetAdVideoInfoAsync(AccessIdentity identity, string videoId, string queryFields)
        {
            var request = identity.Request($"/{videoId}")
                .AddQueryParameter(MetaConst.Fields, queryFields)
                .UseVersion(ApiDefaultVersion);

            return await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaDomain.AdVideo>(request);
        }


        /// <summary>
        /// 获取视频缩略图列表
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="videoId">视频id</param>
        /// <returns></returns>
        public static async Task<MetaPagedDto<MetaDomain.ThumbnailInfo>> GetAdVideoThumbnailsAsync(AccessIdentity identity, string videoId)
        {
            var request = identity.Request($"/{videoId}/thumbnails")
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<MetaDomain.ThumbnailInfo>>(request))!;
        }


        /// <summary>
        ///上传视频，传视频地址
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="accountId">媒体账户id</param>
        /// <param name="fileUrl">视频地址</param>
        /// <param name="fileName">视频名称</param>
        /// <returns></returns>
        public static async Task<MetaOutput.IdResult> AdVideoAddAsync(AccessIdentity identity, string accountId, string fileUrl, string fileName = "")
        {
            var formData = new Dictionary<string, string>
            {
                { "file_url", fileUrl }
            };

            if (!string.IsNullOrEmpty(fileName))
            {
                formData.Add("name", fileName);
            }

            var request = identity.Request($"/{accountId}/advideos")
                .AddBody(formData)
                .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).ConcurrencyPostAsync<MetaOutput.IdResult>(request))!;
        }

    }
}
