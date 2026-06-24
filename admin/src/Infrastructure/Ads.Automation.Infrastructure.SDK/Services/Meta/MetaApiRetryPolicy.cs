using Ads.Automation.Infrastructure.SDK.Exceptions;
using System.Text.Json;
using Ads.Automation.Infrastructure.SDK.Models.Meta.Error;
using Microsoft.Extensions.Logging;

namespace Ads.Automation.Infrastructure.SDK.Services.Meta;

/// <summary>
/// Meta API 指数退避重试策略
/// 调用流程：熔断检查 → 限流预检 → API 调用 → 瞬时错误判断 → 指数退避重试
/// 仅对 HttpResponseException（媒体返回）检查瞬时错误并重试，
/// 其他异常为代码问题，直接抛出不重试
/// </summary>
public class MetaApiRetryPolicy
{
    private readonly ILogger<MetaApiRetryPolicy> _logger;
    private readonly MetaApiRetryOptions _options;
    private readonly MetaApiRateLimiter? _rateLimiter;
    private readonly MetaApiCircuitBreaker? _circuitBreaker;

    /// <summary>
    /// 每次 API 调用完成后触发（成功或失败都会调用）
    /// 参数：accountNo, operation, isRateLimited（code=80004）
    /// </summary>
    public Action<string, string, bool>? OnApiExecuted { get; set; }

    public MetaApiRetryPolicy(
        ILogger<MetaApiRetryPolicy> logger,
        MetaApiRetryOptions? options = null,
        MetaApiRateLimiter? rateLimiter = null,
        MetaApiCircuitBreaker? circuitBreaker = null)
    {
        _logger = logger;
        _options = options ?? new MetaApiRetryOptions();
        _rateLimiter = rateLimiter;
        _circuitBreaker = circuitBreaker;
    }

    /// <summary>
    /// 执行带指数退避重试的 Meta API 调用（带账户追踪 + 熔断 + 限流预检）
    /// </summary>
    public async Task<T> ExecuteAsync<T>(
        Func<Task<T>> apiCall,
        string operation,
        string accountNo,
        CancellationToken ct = default)
    {
        // 全局熔断检查
        if (_circuitBreaker != null && !_circuitBreaker.TryEnter(out var cbReason))
            throw new MetaApiCircuitBreakerOpenException(cbReason);

        var operationType = DetectOperationType(operation);
        var hitRateLimit = false;
        var success = false;

        try
        {
            for (var attempt = 0; attempt <= _options.MaxRetries; attempt++)
            {
                try
                {
                    // 限流预检：在 API 调用前排队等待 token
                    if (_rateLimiter != null)
                        await _rateLimiter.WaitForTokenAsync(operationType, ct);

                    var result = await apiCall();
                    success = true;
                    OnApiExecuted?.Invoke(accountNo, operation, hitRateLimit);
                    return result;
                }
                catch (MetaApiRateLimitException)
                {
                    throw;
                }
                catch (HttpResponseException ex) when (IsTransientMetaError(ex.Response ?? ex.Message) && attempt < _options.MaxRetries)
                {
                    if (IsRateLimitError(ex.Response ?? ex.Message))
                        hitRateLimit = true;

                    var delay = CalculateDelay(attempt);
                    _logger.LogWarning(ex,
                        "Meta API 瞬时异常重试 - 操作:{Op} 第{Attempt}/{Max}次 等待{Delay:0.#}s Code:{Code} Msg:{Msg}",
                        operation, attempt + 1, _options.MaxRetries, delay.TotalSeconds,
                        TryParseMetaError(ex).code, TryParseMetaError(ex).message);

                    await Task.Delay(delay, ct);
                }
            }

            if (_rateLimiter != null)
                await _rateLimiter.WaitForTokenAsync(operationType, ct);

            var finalResult = await apiCall();
            success = true;
            OnApiExecuted?.Invoke(accountNo, operation, hitRateLimit);
            return finalResult;
        }
        finally
        {
            if (success)
                _circuitBreaker?.RecordSuccess();
            else
                _circuitBreaker?.RecordFailure();
        }
    }

    /// <summary>
    /// 执行带指数退避重试的 Meta API 调用（无账户追踪）
    /// </summary>
    public async Task<T> ExecuteAsync<T>(
        Func<Task<T>> apiCall,
        string operation,
        CancellationToken ct = default)
    {
        for (var attempt = 0; attempt <= _options.MaxRetries; attempt++)
        {
            try
            {
                return await apiCall();
            }
            catch (HttpResponseException ex) when (IsTransientMetaError(ex.Response ?? ex.Message) && attempt < _options.MaxRetries)
            {
                var delay = CalculateDelay(attempt);
                _logger.LogWarning(ex,
                    "Meta API 瞬时异常重试 - 操作:{Op} 第{Attempt}/{Max}次 等待{Delay:0.#}s Code:{Code} Msg:{Msg}",
                    operation, attempt + 1, _options.MaxRetries, delay.TotalSeconds,
                    TryParseMetaError(ex).code, TryParseMetaError(ex).message);

                await Task.Delay(delay, ct);
            }
        }

        return await apiCall();
    }

    /// <summary>
    /// 执行带重试的 Meta API 调用（无返回值）
    /// </summary>
    public async Task ExecuteAsync(
        Func<Task> apiCall,
        string operation,
        CancellationToken ct = default)
    {
        await ExecuteAsync(async () =>
        {
            await apiCall();
            return true;
        }, operation, ct);
    }

    /// <summary>
    /// 指数退避计算：min(base × multiplier^attempt, max)
    /// </summary>
    private TimeSpan CalculateDelay(int attempt)
    {
        var seconds = _options.InitialRetryDelaySeconds * Math.Pow(_options.BackoffMultiplier, attempt);
        return TimeSpan.FromSeconds(Math.Min(seconds, _options.MaxRetryDelaySeconds));
    }

    private bool IsTransientMetaError(string rawResponse)
    {
        try
        {
            var jsonStart = rawResponse.IndexOf('{');
            if (jsonStart < 0) return false;

            using var doc = JsonDocument.Parse(rawResponse[jsonStart..]);
            var root = doc.RootElement;

            if (!root.TryGetProperty("error", out var error)) return false;

            // Meta 标记的瞬时错误，直接重试
            if (error.TryGetProperty("is_transient", out var isTransient) && isTransient.GetBoolean())
                return true;

            // 匹配瞬时错误码
            if (error.TryGetProperty("code", out var code) && _options.TransientErrorCodes.Contains(code.GetInt32()))
                return true;

            // 匹配瞬时错误子码（如 code=100 通用错误下的具体可重试类型）
            if (error.TryGetProperty("error_subcode", out var subcode) &&
                _options.TransientErrorSubcodes.Contains(subcode.GetInt32()))
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsRateLimitError(string rawResponse)
    {
        try
        {
            var jsonStart = rawResponse.IndexOf('{');
            if (jsonStart < 0) return false;
            using var doc = JsonDocument.Parse(rawResponse[jsonStart..]);
            return doc.RootElement.TryGetProperty("error", out var error) &&
                   error.TryGetProperty("code", out var code) &&
                   code.GetInt32() == 80004;
        }
        catch
        {
            return false;
        }
    }

    private static (int? code, string? message) TryParseMetaError(Exception ex)
    {
        try
        {
            if (ex is not HttpResponseException httpEx) return (null, ex.Message);

            var raw = httpEx.Response ?? httpEx.Message;
            var jsonStart = raw.IndexOf('{');
            if (jsonStart < 0) return (null, raw);

            var errorDto = JsonSerializer.Deserialize<MetaErrorDto>(raw[jsonStart..]);
            return (errorDto?.error?.code, errorDto?.error?.message);
        }
        catch
        {
            return (null, ex.Message);
        }
    }

    /// <summary>根据操作名称推断 API 类型，用于限流桶选择</summary>
    private static string DetectOperationType(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
            return "read";

        return operation.Contains("创建") || operation.Contains("上传")
            || operation.Contains("复制") || operation.Contains("提交")
            || operation.Contains("删除") || operation.Contains("更新")
            || operation.Contains("修改")
            ? "write"
            : "read";
    }
}
