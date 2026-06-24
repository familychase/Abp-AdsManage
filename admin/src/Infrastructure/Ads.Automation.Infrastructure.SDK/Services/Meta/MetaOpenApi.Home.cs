

namespace Ads.Automation.Infrastructure.Services.Meta
{
    public static partial class MetaOpenApi
    {
        private const string ApiDefaultVersion = "v25.0";

        private static RestClient Client(string url) => RestClientFactory.Get(url);

        private static RestRequest Request(this AccessIdentity identity, string url) => identity.BearerAuthorizationRequest(url);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <param name="pagingNextUrl"></param>
        /// <returns></returns>
        public static Task<MetaPagedDto<T>> PagingNextAsync<T>(AccessIdentity identity, string? pagingNextUrl)
        {
            var request = new RestRequest(pagingNextUrl).AddHeader("Authorization", $"Bearer {identity.AccessToken}");
            return Client(UrlConst.MetaGraphApi).ConcurrencyGetAsync<MetaPagedDto<T>>(request)!;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identity"></param>
        /// <param name="url"></param>
        /// <param name="minDate">时间</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static async Task<List<T>> PagingAllAsync<T>(AccessIdentity identity, string? url, DateTime? minDate = null!, string field = null!)
        {
            string? nextPagingUrl = url;
            var container = new List<T>();

            do
            {
                var response = await PagingNextAsync<T>(identity, nextPagingUrl);

                if (response.data != null)
                    container.AddRange(response.data);
                //加入时间判断，如果时间小于设定时间，直接跳出
                if (minDate != null && field != null)
                {
                    var date = response.data!.Select(obj => (DateTime)(obj!.GetType().GetProperty(field)!.GetValue(obj))!).Min();
                    if (date < minDate) break;
                }

                nextPagingUrl = response.paging?.next ?? string.Empty;

            } while (!string.IsNullOrEmpty(nextPagingUrl));

            return container;
        }

        private static string ToJsonIgnoreNullValue<T>(this T data)
        {
            return JsonSerializer.Serialize(data, IgnoreNullJsonOptions);
        }

        private static readonly JsonSerializerOptions IgnoreNullJsonOptions = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, Converters = { new CustomDateTimeConverter() } };

        /// <summary>
        /// json时间格式  格式化
        /// </summary>
        private class CustomDateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
        {
            private const string Format = "yyyy-MM-dd HH:mm:ss";

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String && reader.TryGetDateTime(out DateTime dateTime))
                    return dateTime;
                else
                    throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(Format));
            }
        }

        /// <summary>
        /// 传入文件地址，获取文件字节流
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<byte[]> HttpUrlBytesGet(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);
                Stream stream = response.Content.ReadAsStream();
                MemoryStream ms = StreamToMemoryStream(stream);
                ms.Seek(0, SeekOrigin.Begin);
                int buffsize = (int)ms.Length;
                byte[] bytes = new byte[buffsize];

                ms.Read(bytes, 0, buffsize);

                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception($"获取本地图片失败！,{ex.StackTrace}");
            }
        }

        //获取内存流
        private static MemoryStream StreamToMemoryStream(Stream instream)
        {
            MemoryStream outstream = new MemoryStream();
            const int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int count = 0;
            while ((count = instream.Read(buffer, 0, bufferLen)) > 0)
            {
                outstream.Write(buffer, 0, count);
            }
            return outstream;
        }
    }
}
