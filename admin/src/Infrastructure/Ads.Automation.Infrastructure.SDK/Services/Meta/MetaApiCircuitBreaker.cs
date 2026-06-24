using Microsoft.Extensions.Logging;

namespace Ads.Automation.Infrastructure.SDK.Services.Meta;

/// <summary>
/// Meta API 全局熔断器（Singleton — 所有 MetaApiRetryPolicy 实例共享状态）
/// 连续失败超过阈值 → 熔断断开 30s → 半开探测 → 成功后恢复
/// </summary>
public class MetaApiCircuitBreaker
{
    private readonly ILogger<MetaApiCircuitBreaker> _logger;
    private readonly object _lock = new();
    private CircuitState _state = CircuitState.Closed;
    private int _failureCount;
    private DateTime _openedAt;

    /// <summary>连续失败多少次触发熔断</summary>
    public int FailureThreshold { get; set; } = 5;

    /// <summary>熔断后冷却时长</summary>
    public TimeSpan BreakDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>当前是否允许请求通过</summary>
    public bool IsOpen
    {
        get
        {
            lock (_lock)
            {
                if (_state == CircuitState.Open)
                {
                    return DateTime.UtcNow - _openedAt < BreakDuration;
                }
                return false;
            }
        }
    }

    public MetaApiCircuitBreaker(ILogger<MetaApiCircuitBreaker> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 检查熔断器并返回是否放行。熔断时返回 false，半开时仅允许一个探测。
    /// </summary>
    public bool TryEnter(out string reason)
    {
        lock (_lock)
        {
            if (_state == CircuitState.Open)
            {
                if (DateTime.UtcNow - _openedAt >= BreakDuration)
                {
                    _state = CircuitState.HalfOpen;
                    _logger.LogInformation("Meta API 熔断器进入半开状态，开始探测恢复");
                }
                else
                {
                    reason = $"熔断器已断开，剩余冷却 {(BreakDuration - (DateTime.UtcNow - _openedAt)).TotalSeconds:0}s";
                    return false;
                }
            }
        }

        reason = string.Empty;
        return true;
    }

    /// <summary>记录一次成功调用</summary>
    public void RecordSuccess()
    {
        lock (_lock)
        {
            if (_state == CircuitState.HalfOpen)
                _logger.LogInformation("Meta API 熔断器探测成功，恢复正常");

            _state = CircuitState.Closed;
            _failureCount = 0;
        }
    }

    /// <summary>记录一次失败调用</summary>
    public void RecordFailure()
    {
        lock (_lock)
        {
            _failureCount++;
            _logger.LogWarning("Meta API 熔断器失败计数: {Count}/{Threshold}",
                _failureCount, FailureThreshold);

            if (_failureCount >= FailureThreshold)
            {
                _state = CircuitState.Open;
                _openedAt = DateTime.UtcNow;
                _logger.LogError(
                    "Meta API 熔断器断开！连续失败 {Count} 次，冷却 {Duration}s",
                    _failureCount, BreakDuration.TotalSeconds);
            }
        }
    }

    private enum CircuitState { Closed, Open, HalfOpen }
}

/// <summary>
/// Meta API 熔断器断开异常
/// </summary>
public class MetaApiCircuitBreakerOpenException : Exception
{
    public MetaApiCircuitBreakerOpenException(string message) : base(message) { }
}
