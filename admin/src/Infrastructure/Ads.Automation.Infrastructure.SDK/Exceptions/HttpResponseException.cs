using RestSharp;
using System.Net;
using System.Text;

namespace Ads.Automation.Infrastructure.SDK.Exceptions
{
    public class HttpResponseException: Exception
    {
        public string Response { get; set; }

        public RestResponse RestResponse { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="restResponse"></param>
        /// <param name="response"></param>
        /// <param name="statusCode"></param>
        /// <param name="ex"></param>
        public HttpResponseException(RestResponse restResponse, string response, HttpStatusCode statusCode, Exception ex) : base($"{ex.Message}. Response:{response}",
        ex.InnerException)
        {
            Response = response;
            RestResponse = restResponse;
            StatusCode = statusCode;
        }
    }
}
