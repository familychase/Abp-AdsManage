using Volo.Abp;
using Volo.Abp.ExceptionHandling;

namespace Ads.Automation.Domain.Shared.Common;

/// <summary>
/// 业务异常，抛出后会被ResponseWrapperMiddleware捕获并返回统一格式的错误响应
/// 业务异常的消息会直接原样返回到客户端
///
/// 实现了 IUserFriendlyException 和 IHasErrorCode 接口，ABP 框架会自动识别：
/// - IUserFriendlyException：将 Message 作为用户友好消息（不显示堆栈）
/// - IHasErrorCode：将 Code 作为错误码（序列化为字符串）
/// </summary>
public class BusinessException : Exception, IUserFriendlyException, IHasErrorCode
{
    /// <summary>
    /// 业务错误码（整型，用于序列化到响应中）
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// IHasErrorCode 接口实现，ABP 会将其序列化为 error.code 字段
    /// </summary>
    string? IHasErrorCode.Code => ErrorCode.ToString();

    /// <summary>
    /// 创建业务异常
    /// </summary>
    /// <param name="message">错误消息，会被返回到客户端</param>
    /// <param name="code">错误码，默认为400（客户端错误）</param>
    public BusinessException(string message, int code = 400)
        : base(message)
    {
        ErrorCode = code;
    }

    /// <summary>
    /// 创建业务异常，包含内部异常信息
    /// </summary>
    /// <param name="message">错误消息，会被返回到客户端</param>
    /// <param name="innerException">内部异常</param>
    /// <param name="code">错误码，默认为400</param>
    public BusinessException(string message, Exception? innerException, int code = 400)
        : base(message, innerException)
    {
        ErrorCode = code;
    }
}
