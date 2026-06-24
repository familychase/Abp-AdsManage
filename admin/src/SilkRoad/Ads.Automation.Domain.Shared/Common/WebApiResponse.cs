namespace Ads.Automation.Api.Common;

/// <summary>
/// 统一API返回模型
/// 无论成功还是失败，都使用相同的结构：
/// - Code: 0表示成功，其他值表示错误码
/// - Message: 成功返回"success"或自定义消息，失败返回错误描述
/// - Data: 成功时返回数据，失败时返回null
/// </summary>
public class WebApiResponse<T>
{
    /// <summary>
    /// 响应代码：0表示成功，其他值表示各种错误
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 响应消息：成功时为"success"或自定义消息，失败时为错误描述
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据：成功时为实际数据，失败时为null
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 成功响应
    /// </summary>
    /// <param name="data">返回的数据</param>
    /// <param name="message">自定义成功消息，默认为"success"</param>
    public static WebApiResponse<T> Success(T? data = default, string message = "success")
    {
        return new WebApiResponse<T>
        {
            Code = 0,
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// 失败响应
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误代码，默认为500</param>
    public static WebApiResponse<T> Fail(string message, int code = 500)
    {
        return new WebApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default
        };
    }
}
