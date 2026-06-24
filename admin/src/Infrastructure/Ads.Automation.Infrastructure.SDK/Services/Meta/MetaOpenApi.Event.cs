
namespace Ads.Automation.Infrastructure.Services.Meta
{
    public partial class MetaOpenApi
    {
        /// <summary>
        /// 像素事件上报
        /// </summary>
        /// <param name="pixelNo">像素编号</param>
        /// <param name="accessToken">token</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static async Task<MetaOutput.EventReportedResult> PixelEventReportedAsync(string pixelNo, string accessToken, MetaDomain.PixelReportedData parameter)
        {
            var request = new RestRequest($"/{pixelNo}/events")
                  .AddQueryParameter("access_token", accessToken)
                  .AddJsonBody(parameter)
                  .UseVersion(ApiDefaultVersion);

            return (await Client(UrlConst.MetaGraphApi).PostAsync<MetaOutput.EventReportedResult>(request))!;
        }
    }
}
