
using Ads.Automation.Infrastructure.SDK.Clients;
using Ads.Automation.Infrastructure.SDK.Consts;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Error;
using Ads.Automation.Infrastructure.SDK.Models.Meta.General;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Ads.Automation.Infrastructure.SDK.Extensions;
public static class RestRequestExtension
{
    private const string MetaPlatform = "META";
    private const string TikTokPlatform = "TIKTOK";

    /// <summary>
    /// HTTP 请求回调：accountNo, method(GET/POST/DELETE), points, isRateLimited
    /// </summary>
    public static Action<string, string, int, bool>? OnHttpRequest;

    /// <summary>
    /// 从请求 URL 中提取账户编号
    /// </summary>
    private static string? ExtractAccountNo(RestRequest request)
    {
        var resource = request.Resource;
        var match = Regex.Match(resource, @"act_(\d+)");
        return match.Success ? $"act_{match.Groups[1].Value}" : null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static RestRequest UseVersion(this RestRequest request, string version)
    {
        request.Resource = $"/{version}/{request.Resource.TrimStart('/')}";
        return request;
    }


    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="method"></param>
    /// <param name="retryCount">重试次数</param>
    private static async Task<RestResponse<T>> ConcurrencyExecuteAsync<T>(this RestClient client, RestRequest request, Method method, int retryCount = 5)
    {
        // 获取Host，获取请求媒体
        var host = client.BuildUri(request).Host;
        var mediaPlatform = host == "graph.facebook.com" ? MetaPlatform : (host.Contains("tiktok.com") ? TikTokPlatform : string.Empty);

        try
        {
            return await ConcurrencyExecuteAsync<T>(client, request, method, mediaPlatform);
        }
        catch (HttpResponseException ex)
        {
            // 目前只有facebook|TikTok需要加入重试，重试次数归 0，也是直接跳出
            if (string.IsNullOrEmpty(mediaPlatform) || retryCount == 0) throw;
            // 判断META是否需要重试
            if (mediaPlatform == MetaPlatform)
            {
                var needRetry = await JudgeFacebookNeedRetry(request, ex);
                if (!needRetry) throw;
            }
            // 判断TIKTOK是否需要重试
            else if (mediaPlatform == TikTokPlatform)
            {
                var needRetry = await JudgeTikTokNeedRetry(request, ex);
                if (!needRetry) throw;
            }

            return await ConcurrencyExecuteAsync<T>(client, request, method, retryCount - 1);
        }
    }


    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="method"></param>
    /// <param name="mediaPlatform">请求的媒体平台</param>
    /// <returns></returns>
    public static async Task<RestResponse<T>> ConcurrencyExecuteAsync<T>(this RestClient client, RestRequest request, Method method, string mediaPlatform)
    {
        var response = await client.ExecuteAsync(request, method);

        // 每次 HTTP 请求都触发统计回调（仅 Meta 平台）
        if (mediaPlatform == MetaPlatform)
        {
            var accountNo = ExtractAccountNo(request);
            if (accountNo != null)
            {
                var isRateLimit = IsRateLimitResponse(response);
                var points = method switch
                {
                    Method.Get => 1,
                    Method.Post or Method.Delete or Method.Put => 3,
                    _ => 1
                };
                var methodName = method.ToString().ToUpperInvariant();
                OnHttpRequest?.Invoke(accountNo, methodName, points, isRateLimit);
            }
        }

        // Step 1: 请求成功, 返回结果数据; 
        if (response is { ResponseStatus: ResponseStatus.Completed, IsSuccessStatusCode: true })
        {
            // tiktok平台，请求失败，则进行替换
            if (mediaPlatform == TikTokPlatform && response.Content != null)
            {
                response.Content = response.Content.Replace("\"code\": 0", "\"code\":0");
                if (!response.Content.Contains("\"code\":0"))
                {
                    response.Content = response.Content.Replace("\"data\": {}", "\"data\": null");

                    if (response.Content.Contains("App 7055107885017071618 reaches the QPS limit 50"))
                    {
                        throw new Exception("App 7055107885017071618 reaches the QPS limit 50");
                    }
                }
            }

            return client.Deserialize<T>(response);
        }

        // Step 2: 判断是否是永久重定向
        if (response.StatusCode == HttpStatusCode.PermanentRedirect)
        {
            var redirectUrl = new Uri(response.Headers!.First(c => c.Name == "Location").Value!.ToString()!);
            var newClient = RestClientFactory.Get(redirectUrl);
            var newRequest = new RestRequest(redirectUrl.PathAndQuery);

            foreach (var parameter in request.Parameters)
            {
                if (parameter.Type == ParameterType.HttpHeader)
                {
                    newRequest.AddParameter(parameter.Name, parameter.Value!, parameter.Type, parameter.Encode);
                }
            }

            return await newClient.ConcurrencyExecuteAsync<T>(newRequest, method);
        }

        if (response.Content != null && !string.IsNullOrWhiteSpace(response.Content))
        {
            throw new HttpResponseException(response, response.Content, response.StatusCode, response.ErrorException!);
        }

        throw response.ErrorException!;
    }

    /// <summary>
    /// 检测是否为限流响应（code=80004）
    /// </summary>
    private static bool IsRateLimitResponse(RestResponse response)
    {
        if (string.IsNullOrWhiteSpace(response.Content)) return false;
        try
        {
            using var doc = JsonDocument.Parse(response.Content);
            return doc.RootElement.TryGetProperty("error", out var error) &&
                   error.TryGetProperty("code", out var code) &&
                   code.GetInt32() == 80004;
        }
        catch { return false; }
    }

    /// <summary>
    /// 判断Facebook是否需要重试
    /// </summary>
    /// <param name="request">请求对象</param>
    /// <param name="ex">异常信息</param>
    /// <returns></returns>
    private static async Task<bool> JudgeFacebookNeedRetry(RestRequest request, HttpResponseException ex)
    {
        try
        {
            // 媒体内部服务错误
            if (ex.Message.Contains("Service Unavailable")) return true;

            // 获取错误信息
            var message = ex.Message[ex.Message.IndexOf("{")..];
            var errorObject = JsonSerializer.Deserialize<MetaErrorDto>(message);

            if (errorObject != null && errorObject.error != null)
            {
                switch (errorObject.error.code)
                {
                    case 1:
                        {
                            // 请求数目过多处理
                            if (errorObject.error.message.StartsWith("Please reduce the amount of data you're asking for"))
                            {
                                var parameter = request.Parameters.FirstOrDefault(p => p.Name == MetaConst.Limit);
                                if (parameter != null)
                                {
                                    request.Parameters.RemoveParameter(MetaConst.Limit);
                                    request.AddQueryParameter(MetaConst.Limit, Convert.ToInt32(parameter!.Value) / 2);
                                }

                                return true;
                            }

                            // 未知错误，进行重试，等待时间2s
                            if (errorObject.error.message.StartsWith("An unknown error"))
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                return true;
                            }
                        }
                        break;
                    case 2:
                        {
                            // 未知错误，进行重试，等待时间2s
                            if (errorObject.error.message.StartsWith("An unexpected error has occurred"))
                            {
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                return true;
                            }
                        }
                        break;
                }
            }
        }
        catch
        {

        }

        // 未找到媒体服务
        var errorMsg = "<!DOCTYPE html";
        if (ex.Message.Contains(errorMsg) || (ex.InnerException != null && ex.InnerException.Message.Contains(errorMsg)))
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return true;
        }

        return false;
    }



    /// <summary>
    /// 判断Facebook是否需要重试
    /// </summary>
    /// <param name="request">请求对象</param>
    /// <param name="ex">异常信息</param>
    /// <returns></returns>
    private static async Task<bool> JudgeTikTokNeedRetry(RestRequest request, HttpResponseException ex)
    {
        var errorMsgs = new List<string>()
        {
            "please retry",
            "try again",
            "Gateway Time-out",
            "Internal Time",
            "Internal service",
            "reaches the QPS limit"
        };

        foreach (var errorMsg in errorMsgs)
        {
            if (ex.Message.Contains(errorMsg) || (ex.InnerException != null && ex.InnerException.Message.Contains(errorMsg)))
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<T?> ConcurrencyPostAsync<T>(this RestClient client, RestRequest request)
    {
        return (await ConcurrencyExecuteAsync<T>(client, request, Method.Post)).Data;
    }

    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<T?> ConcurrencyPutAsync<T>(this RestClient client, RestRequest request)
    {
        return (await ConcurrencyExecuteAsync<T>(client, request, Method.Put)).Data;
    }

    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<T?> ConcurrencyDeleteAsync<T>(this RestClient client, RestRequest request)
    {
        return (await ConcurrencyExecuteAsync<T>(client, request, Method.Delete)).Data;
    }

    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<T?> ConcurrencyGetAsync<T>(this RestClient client, RestRequest request)
    {
        return (await ConcurrencyExecuteAsync<T>(client, request, Method.Get)).Data;
    }

    /// <summary>
    /// 并发控制类请求, 使用此函数将收到后台令牌桶的并发控制. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<MetaPagedDto<T>?> MetaConcurrencyGetAsync<T>(this RestClient client, RestRequest request)
    {
        var data = (await ConcurrencyExecuteAsync<MetaPagedDto<T>>(client, request, Method.Get)).Data;

        // 处理META的分页逻辑，如果没有返回下一页地址，但是有下一页的码，则进行手动赋值
        if (data != null && data.paging != null && data.paging.cursors != null && string.IsNullOrEmpty(data.paging.next))
        {
            if (!string.IsNullOrEmpty(data.paging.cursors.after) &&
                !data.paging.cursors.after.Equals(data.paging.cursors.before))
            {
                var requestUrl = client.BuildUri(request).ToString();

                data.paging.next = $"{requestUrl}&after={data.paging.cursors.after}";
            }
        }

        return data;
    }
}