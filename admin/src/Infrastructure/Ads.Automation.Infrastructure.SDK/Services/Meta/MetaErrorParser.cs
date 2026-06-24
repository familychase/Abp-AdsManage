// ReSharper disable InconsistentNaming

using System.Text.Json;
using Ads.Automation.Domain.Shared.Localization;
using Ads.Automation.Infrastructure.SDK.Exceptions;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Error;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Ads.Automation.Infrastructure.SDK.Services.Meta;

/// <summary>
/// Meta API error parser - classifies errors by code and generates user-friendly messages
/// </summary>
public class MetaErrorParser
{
    private readonly ILogger<MetaErrorParser> _logger;
    private readonly IStringLocalizer<AdsAutomationResource> _localizer;

    public MetaErrorParser(
        ILogger<MetaErrorParser> logger,
        IStringLocalizer<AdsAutomationResource> localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }

    /// <summary>
    /// Parse Meta API error from raw response message
    /// </summary>
    public MetaErrorParseResult Parse(string rawMessage)
    {
        try
        {
            var jsonStart = rawMessage.IndexOf('{');
            if (jsonStart < 0)
                return new MetaErrorParseResult { UserMessage = rawMessage };

            var json = rawMessage[jsonStart..];
            if (json.EndsWith(")"))
                json = json[..^1];

            var errorDto = JsonSerializer.Deserialize<MetaErrorDto>(json);
            if (errorDto?.error == null)
                return new MetaErrorParseResult { UserMessage = rawMessage };

            var err = errorDto.error;
            var userMessage = err.message ?? "Unknown error";

            if (!string.IsNullOrEmpty(err.error_user_msg))
                userMessage = err.error_user_msg;

            return new MetaErrorParseResult
            {
                Code = err.code,
                SubCode = err.error_subcode,
                Message = err.message ?? string.Empty,
                UserMessage = userMessage,
                UserTitle = err.error_user_title ?? string.Empty,
            };
        }
        catch
        {
            return new MetaErrorParseResult { UserMessage = rawMessage };
        }
    }

    /// <summary>
    /// Check if error is retryable transient error
    /// </summary>
    public bool IsRetryableError(MetaErrorParseResult result)
    {
        return result.Code switch
        {
            1 => true,
            2 => true,
            100 => result.SubCode is 2238011 or 1363011 or 2490499,
            389 => true,
            _ => false
        };
    }

    /// <summary>
    /// Get retry delay for the error
    /// </summary>
    public TimeSpan GetRetryDelay(MetaErrorParseResult result)
    {
        return result.Code switch
        {
            1 => TimeSpan.FromSeconds(30),
            2 => TimeSpan.FromSeconds(30),
            100 => result.SubCode switch
            {
                2238011 => TimeSpan.FromSeconds(30),
                1363011 => TimeSpan.FromSeconds(5),
                2490499 => TimeSpan.FromSeconds(5),
                _ => TimeSpan.FromSeconds(1)
            },
            389 => TimeSpan.FromSeconds(10),
            _ => TimeSpan.Zero
        };
    }

    /// <summary>
    /// Generate user-friendly error message
    /// </summary>
    public string GetUserFriendlyMessage(MetaErrorParseResult result, string context = "")
    {
        var sb = new System.Text.StringBuilder();

        if (!string.IsNullOrEmpty(context))
            sb.Append('[').Append(context).Append("] ");

        var codeMessage = result.Code switch
        {
            1 => _localizer["MetaError:UnknownError"],
            2 => _localizer["MetaError:RequestTooFrequent"],
            100 => result.SubCode switch
            {
                2238011 => _localizer["MetaError:InstagramAccountCreating"],
                1363011 => _localizer["MetaError:VideoProcessingError"],
                2490499 => _localizer["MetaError:PlacementNotSupported"],
                _ => _localizer["MetaError:InvalidParameter"]
            },
            389 => _localizer["MetaError:VideoFileUnreachable"],
            80004 => _localizer["MetaError:RateLimitReached"],
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(codeMessage))
            sb.Append(codeMessage);

        if (!string.IsNullOrEmpty(result.UserMessage))
        {
            if (sb.Length > 0) sb.Append(": ");
            sb.Append(result.UserMessage);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extract user-friendly error message from exception
    /// </summary>
    public string ExtractErrorMessage(Exception ex)
    {
        if (ex is HttpResponseException httpEx)
        {
            try
            {
                var rawMessage = httpEx.Response ?? httpEx.Message;
                var result = Parse(rawMessage);

                return result.Code switch
                {
                    _ when rawMessage.Contains("GatewayTimeout") => _localizer["MetaError:GatewayTimeout"],

                    80004 => _localizer["MetaError:RateLimitReachedWait"],

                100 => result.SubCode switch
                {
                    2238011 => _localizer["MetaError:InstagramAccountCreating30s"],
                    1363011 => _localizer["MetaError:VideoProcessingError"],
                    2490499 => _localizer["MetaError:PlacementAutoSkipped"],
                    1885316 => _localizer["MetaError:AccountDisabled"],
                    1885985 => _localizer["MetaError:MultilingualLocationNotSupported"],
                    _ => string.Format(_localizer["MetaError:InvalidParameterWithMessage"], result.UserMessage)
                },

                    389 => _localizer["MetaError:VideoFetchFailed"],

                    1 or 2 => string.Format(_localizer["MetaError:ServiceUnavailable"], result.Code),

                    _ => GetUserFriendlyMessage(result)
                };
            }
            catch { }

            return string.Format(_localizer["MetaError:ApiError"], httpEx.Message);
        }

        if (ex is HttpRequestException)
            return _localizer["MetaError:NetworkRequestFailed"];

        if (ex is TaskCanceledException or TimeoutException)
            return _localizer["MetaError:RequestTimeout"];

        return string.IsNullOrEmpty(ex.Message) ? ex.GetType().Name : ex.Message;
    }
}

/// <summary>
/// Meta API error parse result
/// </summary>
public class MetaErrorParseResult
{
    /// <summary>Error code</summary>
    public int Code { get; set; }

    /// <summary>Sub error code</summary>
    public int? SubCode { get; set; }

    /// <summary>Original message</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>User-friendly message</summary>
    public string UserMessage { get; set; } = string.Empty;

    /// <summary>User-friendly title</summary>
    public string UserTitle { get; set; } = string.Empty;

    /// <summary>Whether transient error</summary>
    public bool? IsTransient { get; set; }
}
