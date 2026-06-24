namespace Ads.Automation.Domain.Shared.Common;

/// <summary>
/// API error code definitions.
/// Error messages are resolved via IStringLocalizer&lt;AdsAutomationResource&gt; at runtime.
/// </summary>
public static class ErrorCodes
{
    // Success
    /// <summary>
    /// Success - Code: 0
    /// </summary>
    public const int Success = 0;

    // Client errors 4xx
    /// <summary>
    /// Bad Request - Code: 400
    /// </summary>
    public const int BadRequest = 400;

    /// <summary>
    /// Unauthorized - Code: 401
    /// </summary>
    public const int Unauthorized = 401;

    /// <summary>
    /// Forbidden - Code: 403
    /// </summary>
    public const int Forbidden = 403;

    /// <summary>
    /// Resource Not Found - Code: 404
    /// </summary>
    public const int NotFound = 404;

    /// <summary>
    /// Conflict - Code: 409
    /// </summary>
    public const int Conflict = 409;

    // Server errors 5xx
    /// <summary>
    /// System Error - Code: 500
    /// </summary>
    public const int SystemError = 500;

    /// <summary>
    /// Service Unavailable - Code: 503
    /// </summary>
    public const int ServiceUnavailable = 503;

    // Business errors 10xx
    /// <summary>
    /// User Not Found - Code: 1001
    /// </summary>
    public const int UserNotFound = 1001;

    /// <summary>
    /// User Already Exists - Code: 1002
    /// </summary>
    public const int UserAlreadyExists = 1002;

    /// <summary>
    /// Password Error - Code: 1003
    /// </summary>
    public const int PasswordError = 1003;

    /// <summary>
    /// Permission Denied - Code: 1004
    /// </summary>
    public const int PermissionDenied = 1004;
}
