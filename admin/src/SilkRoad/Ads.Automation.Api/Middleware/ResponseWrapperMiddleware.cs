using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BusinessException = Ads.Automation.Domain.Shared.Common.BusinessException;

namespace Ads.Automation.Api.Middleware;

/// <summary>
/// Global response wrapper middleware.
/// Wraps all JSON responses into unified format { code, message, data } and catches exceptions.
/// </summary>
public class ResponseWrapperMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ResponseWrapperMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IStringLocalizer<AdsAutomationResource> _localizer;

    public ResponseWrapperMiddleware(
        RequestDelegate next,
        ILogger<ResponseWrapperMiddleware> logger,
        IWebHostEnvironment environment,
        IStringLocalizer<AdsAutomationResource> localizer)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
        _localizer = localizer;
    }

    /// <summary>
    /// Write error response to stream
    /// </summary>
    private static async Task WriteErrorResponseAsync(
        Stream stream,
        HttpContext context,
        string message,
        int code)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = StatusCodes.Status200OK;

        var response = WebApiResponse<object>.Fail(message, code);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        var bytes = Encoding.UTF8.GetBytes(json);
        await stream.WriteAsync(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Detect ABP framework error response format { "error": { "code": "...", "message": "..." } }
    /// </summary>
    private bool TryParseAbpError(string bodyText, out int code, out string message)
    {
        code = 500;
        message = _localizer["SystemErrorShort"];
        try
        {
            using var doc = JsonDocument.Parse(bodyText);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return false;
            if (!doc.RootElement.TryGetProperty("error", out var errorElement)) return false;
            if (errorElement.ValueKind != JsonValueKind.Object) return false;

            if (errorElement.TryGetProperty("message", out var msgProp))
            {
                message = msgProp.GetString() ?? _localizer["SystemErrorShort"];
            }

            if (errorElement.TryGetProperty("code", out var codeProp))
            {
                var codeStr = codeProp.GetString();
                if (!string.IsNullOrEmpty(codeStr) && int.TryParse(codeStr, out var parsedCode))
                {
                    code = parsedCode;
                }
                else if (!string.IsNullOrEmpty(codeStr))
                {
                    code = 400;
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 判断响应体是否为有效的 JSON 格式
    /// </summary>
    private static bool IsValidJson(string bodyText)
    {
        if (string.IsNullOrWhiteSpace(bodyText)) return false;
        var trimmed = bodyText.TrimStart();
        if (trimmed.Length == 0) return false;
        // JSON 必须以 { 或 [ 开头
        if (trimmed[0] != '{' && trimmed[0] != '[') return false;

        try
        {
            using var doc = JsonDocument.Parse(bodyText);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 检查响应是否已经被包装
    /// </summary>
    private static bool IsAlreadyWrapped(string bodyText)
    {
        try
        {
            using var doc = JsonDocument.Parse(bodyText);
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                bool hasCode = false, hasMessage = false, hasData = false;
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    var name = prop.Name;
                    if (string.Equals(name, "code", StringComparison.OrdinalIgnoreCase)) hasCode = true;
                    if (string.Equals(name, "message", StringComparison.OrdinalIgnoreCase)) hasMessage = true;
                    if (string.Equals(name, "data", StringComparison.OrdinalIgnoreCase)) hasData = true;
                }
                return hasCode && hasMessage && hasData;
            }
        }
        catch
        {
            // 解析失败则认为未被包装
        }

        return false;
    }

    /// <summary>
    /// 包装JSON响应
    /// </summary>
    private static string WrapResponse(string bodyText)
    {
        object? parsed = null;
        try
        {
            using var doc = JsonDocument.Parse(bodyText);
            var root = doc.RootElement;

            // 处理分页数据：兼容Items/TotalCount和items/totalCount
            bool hasItems = root.TryGetProperty("items", out var itemsProp) ||
                           root.TryGetProperty("Items", out itemsProp);
            bool hasTotal = root.TryGetProperty("totalCount", out var totalProp) ||
                           root.TryGetProperty("TotalCount", out totalProp);

            if (hasItems && hasTotal)
            {
                var dataDict = new Dictionary<string, object>
                {
                    ["items"] = itemsProp.Clone(),
                    ["totalCount"] = totalProp.Clone()
                };
                parsed = dataDict;
            }
            else
            {
                parsed = JsonSerializer.Deserialize<object>(bodyText, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch
        {
            parsed = bodyText;
        }

        var wrapper = new
        {
            code = 0,
            message = "success",
            data = parsed
        };

        return JsonSerializer.Serialize(wrapper, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        });
    }

    /// <summary>
    /// Build developer error message with inner exception unwrapping.
    /// </summary>
    private string BuildDeveloperErrorMessage(Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{ex.GetType().Name}: {ex.Message}");

        // Unwrap DbUpdateException to reveal the real database error
        if (ex is Microsoft.EntityFrameworkCore.DbUpdateException dbEx && dbEx.InnerException != null)
        {
            sb.AppendLine($"  → {dbEx.InnerException.GetType().Name}: {dbEx.InnerException.Message}");
        }
        else if (ex.InnerException != null)
        {
            sb.AppendLine($"  → Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
        }

        sb.Append($"{_localizer["StackTracePrefix"]}{ex.StackTrace}");
        return sb.ToString();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await _next(context);

            // Process response
            buffer.Seek(0, SeekOrigin.Begin);
            var bodyText = await new StreamReader(buffer, Encoding.UTF8).ReadToEndAsync();

            // Empty response — pass through
            if (string.IsNullOrWhiteSpace(bodyText))
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBodyStream);
                return;
            }

            // Non-JSON response — pass through
            var contentType = context.Response.ContentType ?? string.Empty;
            if (!contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase)
                && !contentType.Contains("text/json", StringComparison.OrdinalIgnoreCase)
                && !IsValidJson(bodyText))
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBodyStream);
                return;
            }

            // Swagger/OpenAPI — pass through
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.Contains("/swagger/", StringComparison.OrdinalIgnoreCase) ||
                path.Contains("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBodyStream);
                return;
            }

            // Already wrapped — pass through
            if (IsAlreadyWrapped(bodyText))
            {
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBodyStream);
                return;
            }

            // ABP error response — convert to unified format
            if (TryParseAbpError(bodyText, out var abpErrorCode, out var abpErrorMessage))
            {
                await WriteErrorResponseAsync(originalBodyStream, context, abpErrorMessage, abpErrorCode);
                return;
            }

            // Wrap response
            context.Response.ContentType = "application/json; charset=utf-8";
            var wrappedJson = WrapResponse(bodyText);
            var wrappedBytes = Encoding.UTF8.GetBytes(wrappedJson);
            await originalBodyStream.WriteAsync(wrappedBytes, 0, wrappedBytes.Length);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Business exception: [{Code}] {Message}", ex.ErrorCode, ex.Message);
            await WriteErrorResponseAsync(originalBodyStream, context, ex.Message, ex.ErrorCode);
        }
        catch (Volo.Abp.BusinessException ex)
        {
            _logger.LogWarning(ex, "ABP business exception: [{Code}] {Message}", ex.Code, ex.Message);
            await WriteErrorResponseAsync(originalBodyStream, context, ex.Message, 400);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System exception caught by ResponseWrapperMiddleware");

            string errorMessage;
            if (_environment.IsDevelopment())
            {
                errorMessage = BuildDeveloperErrorMessage(ex);
            }
            else
            {
                errorMessage = _localizer["NetworkException"];
            }

            await WriteErrorResponseAsync(originalBodyStream, context, errorMessage, ErrorCodes.SystemError);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}
