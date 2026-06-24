using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Ads.Automation.Infrastructure.SDK.Clients
{
    public class HttpClientWithRetry
    {
        /// <summary>
        /// 底层HTTP客户端实例
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 最大重试次数
        /// </summary>
        private readonly int _maxRetries;

        /// <summary>
        /// 重试间隔延迟时间
        /// </summary>
        private readonly TimeSpan _retryDelay;

        /// <summary>
        /// 初始化带重试机制的HTTP客户端
        /// </summary>
        /// <param name="httpClient">底层HTTP客户端实例</param>
        /// <param name="maxRetries">最大重试次数，默认为3次</param>
        /// <param name="retryDelay">重试延迟时间，默认为1秒</param>
        /// <exception cref="ArgumentNullException">当httpClient为null时抛出</exception>
        public HttpClientWithRetry(HttpClient httpClient, int maxRetries = 3, TimeSpan? retryDelay = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _maxRetries = maxRetries;
            _retryDelay = retryDelay ?? TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// 执行带重试机制的POST请求
        /// </summary>
        /// <param name="uri">目标URI地址</param>
        /// <param name="content">HTTP请求内容</param>
        /// <returns>HTTP响应消息</returns>
        /// <exception cref="HttpRequestException">当达到最大重试次数仍失败时抛出</exception>
        /// <exception cref="ArgumentNullException">当uri或content为null时抛出</exception>
        /// <remarks>
        /// 自动重试条件：
        /// - 5xx服务器错误状态码
        /// - 429 Too Many Requests状态码
        /// - 网络请求异常
        /// - 请求超时异常
        /// </remarks>
        public async Task<HttpResponseMessage> PostWithRetryAsync(string uri, HttpContent content)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri));
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            return await SendAsync(uri, content);
        }

        /// <summary>
        /// 执行带重试机制的GET请求
        /// </summary>
        /// <param name="uri">目标URI地址</param>
        /// <returns>HTTP响应消息</returns>
        /// <exception cref="HttpRequestException">当达到最大重试次数仍失败时抛出</exception>
        /// <exception cref="ArgumentNullException">当uri为null或空时抛出</exception>
        /// <remarks>
        /// 自动重试条件：
        /// - 5xx服务器错误状态码
        /// - 429 Too Many Requests状态码
        /// - 网络请求异常
        /// - 请求超时异常
        /// </remarks>
        public async Task<HttpResponseMessage> GetWithRetryAsync(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri));

            return await SendAsync(uri);
        }

        /// <summary>
        /// 核心发送方法，实现重试逻辑
        /// </summary>
        /// <param name="uri">目标URI地址</param>
        /// <param name="content">HTTP请求内容，可为null用于GET请求</param>
        /// <returns>HTTP响应消息</returns>
        /// <exception cref="HttpRequestException">当达到最大重试次数仍失败时抛出</exception>
        private async Task<HttpResponseMessage> SendAsync(string uri, HttpContent? content = null)
        {
            int retryCount = 0;
            Exception? lastException = null;

            while (retryCount < _maxRetries)
            {
                try
                {
                    // 创建HTTP请求消息
                    var request = new HttpRequestMessage()
                    {
                        Method = content == null ? HttpMethod.Get : HttpMethod.Post,
                        RequestUri = new Uri(uri)
                    };

                    // 设置请求内容（仅POST请求）
                    if (content != null)
                        request.Content = content;

                    // 发送HTTP请求
                    var response = await _httpClient.SendAsync(request);

                    // 如果请求成功（状态码 2xx），直接返回
                    if (response.IsSuccessStatusCode)
                    {
                        return response;
                    }

                    // 如果状态码是 429（Too Many Requests）或 5xx（服务器错误），可以重试
                    if ((int)response.StatusCode >= 500 || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        retryCount++;
                        Console.WriteLine($"Attempt {retryCount} failed with status code {response.StatusCode}. Retrying...");
                        await Task.Delay(_retryDelay);
                        continue;
                    }

                    // 其他状态码（如 400、401、404 等）不重试，直接返回
                    return response;
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    retryCount++;
                    Console.WriteLine($"Attempt {retryCount} failed: {ex.Message}. Retrying...");
                    await Task.Delay(_retryDelay);
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                {
                    lastException = ex;
                    retryCount++;
                    Console.WriteLine($"Attempt {retryCount} timed out. Retrying...");
                    await Task.Delay(_retryDelay);
                }
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //throw new HttpRequestException($"Max retries ({_maxRetries}) exceeded.", lastException);


        }
    }
}
