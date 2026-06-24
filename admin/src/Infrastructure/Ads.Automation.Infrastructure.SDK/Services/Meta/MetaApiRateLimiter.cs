using Ads.Automation.Infrastructure.Caching.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Ads.Automation.Infrastructure.SDK.Services.Meta;

/// <summary>
/// Meta API Redis 令牌桶限流器
/// 在读/写操作前通过 Redis Lua 脚本原子性检查并消耗 token，
/// 避免打到 Meta 后才收到 80004 限流错误
/// </summary>
public class MetaApiRateLimiter
{
    private readonly IRedisConnectionProvider _redis;
    private readonly MetaApiRateLimitOptions _options;
    private readonly ILogger<MetaApiRateLimiter> _logger;

    // Lua 脚本：原子性令牌桶检查与消耗
    // KEYS[1]: 令牌桶 key
    // ARGV[1]: 请求的 token 数
    // ARGV[2]: 桶最大容量
    // ARGV[3]: 补充速率（tokens/second）
    // ARGV[4]: 当前 Unix 时间戳（秒）
    // 返回: [allowed (1/0), remaining_tokens, retry_after_ms]
    private const string TokenBucketScript = """
        local bucket_key = KEYS[1]
        local requested = tonumber(ARGV[1])
        local capacity = tonumber(ARGV[2])
        local refill_rate = tonumber(ARGV[3])
        local now_sec = tonumber(ARGV[4])

        -- 读取当前桶状态
        local bucket = redis.call('HMGET', bucket_key, 'tokens', 'last_refill')
        local tokens = tonumber(bucket[1]) or capacity
        local last_refill = tonumber(bucket[2]) or now_sec

        -- 计算需要补充的 token
        local elapsed = math.max(0, now_sec - last_refill)
        local refill = math.floor(elapsed * refill_rate)
        tokens = math.min(capacity, tokens + refill)

        -- 判断是否允许
        local allowed = 0
        local retry_after_ms = 0
        if tokens >= requested then
            tokens = tokens - requested
            allowed = 1
        else
            -- 计算需要等待多少秒才能积累足够的 token
            local needed = requested - tokens
            retry_after_ms = math.ceil((needed / refill_rate) * 1000)
        end

        -- 更新桶状态，设置 TTL 防止 key 堆积
        redis.call('HMSET', bucket_key, 'tokens', tokens, 'last_refill', now_sec)
        redis.call('EXPIRE', bucket_key, 300)

        return {allowed, tokens, retry_after_ms}
        """;

    public MetaApiRateLimiter(
        IRedisConnectionProvider redis,
        MetaApiRateLimitOptions options,
        ILogger<MetaApiRateLimiter> logger)
    {
        _redis = redis;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// 尝试消耗读操作 token（GET 请求），限流时返回需要等待的毫秒数
    /// </summary>
    /// <returns>(allowed, retryAfterMs) — allowed=true 表示放行，否则 retryAfterMs 为建议等待时间</returns>
    public Task<(bool allowed, int retryAfterMs)> TryAcquireReadAsync()
        => TryAcquireAsync("read", _options.ReadBucketCapacity, _options.ReadRefillRatePerSecond);

    /// <summary>
    /// 尝试消耗写操作 token（POST/PUT/DELETE），限流时返回需要等待的毫秒数
    /// </summary>
    public Task<(bool allowed, int retryAfterMs)> TryAcquireWriteAsync()
        => TryAcquireAsync("write", _options.WriteBucketCapacity, _options.WriteRefillRatePerSecond);

    /// <summary>
    /// 等待直到获取到 token 或超时
    /// </summary>
    public async Task WaitForTokenAsync(string operationType, CancellationToken ct = default)
    {
        if (!_options.Enabled)
            return;

        var (capacity, rate) = operationType switch
        {
            "read" => (_options.ReadBucketCapacity, _options.ReadRefillRatePerSecond),
            "write" => (_options.WriteBucketCapacity, _options.WriteRefillRatePerSecond),
            _ => (_options.ReadBucketCapacity, _options.ReadRefillRatePerSecond)
        };

        var deadline = DateTimeOffset.UtcNow.AddSeconds(_options.MaxWaitSeconds);
        var attempt = 0;

        while (DateTimeOffset.UtcNow < deadline)
        {
            attempt++;
            var (allowed, retryAfterMs) = await TryAcquireAsync(
                operationType, capacity, rate);

            if (allowed)
            {
                if (attempt > 1)
                {
                    _logger.LogInformation(
                        "Meta API 限流排队完成: OperationType={OpType}, Attempts={Attempts}",
                        operationType, attempt);
                }
                return;
            }

            // 等待后重试（至少 100ms，防止空转）
            var waitMs = Math.Max(retryAfterMs, 100);
            _logger.LogDebug(
                "Meta API 限流排队中: OperationType={OpType}, RetryAfter={RetryAfterMs}ms, Attempt={Attempt}",
                operationType, retryAfterMs, attempt);

            await Task.Delay(waitMs, ct);
        }

        throw new MetaApiRateLimitException(
            $"Meta API 限流等待超时（{_options.MaxWaitSeconds}s），操作类型: {operationType}");
    }

    /// <summary>检查当前剩余 token 数（不消耗）</summary>
    public async Task<(long readTokens, long writeTokens)> GetAvailableTokensAsync()
    {
        var readKey = BuildKey("read");
        var writeKey = BuildKey("write");
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var readBucket = await _redis.Database.HashGetAsync(readKey, "tokens");
        var writeBucket = await _redis.Database.HashGetAsync(writeKey, "tokens");
        var readLastRefill = await _redis.Database.HashGetAsync(readKey, "last_refill");
        var writeLastRefill = await _redis.Database.HashGetAsync(writeKey, "last_refill");

        long ParseAndRefill(RedisValue tokensVal, RedisValue lastRefillVal, int capacity, double rate)
        {
            var tokens = tokensVal.IsInteger ? (long)tokensVal : capacity;
            var lastRefill = lastRefillVal.IsInteger ? (long)lastRefillVal : now;
            var elapsed = Math.Max(0, now - lastRefill);
            return Math.Min(capacity, tokens + (long)Math.Floor(elapsed * rate));
        }

        return (
            ParseAndRefill(readBucket, readLastRefill, _options.ReadBucketCapacity, _options.ReadRefillRatePerSecond),
            ParseAndRefill(writeBucket, writeLastRefill, _options.WriteBucketCapacity, _options.WriteRefillRatePerSecond)
        );
    }

    private async Task<(bool allowed, int retryAfterMs)> TryAcquireAsync(
        string bucketType, int capacity, double refillRate)
    {
        try
        {
            var key = BuildKey(bucketType);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // StackExchange.Redis 内部通过 SHA1 自动缓存 Lua 脚本，直接传原始脚本即可
            var result = await _redis.Database.ScriptEvaluateAsync(
                TokenBucketScript,
                new RedisKey[] { key },
                new RedisValue[] { 1, capacity, refillRate, now });

            var values = (RedisResult[])result!;
            var allowed = (int)values[0] == 1;
            var retryAfterMs = (int)values[2];

            return (allowed, retryAfterMs);
        }
        catch (Exception ex)
        {
            // Redis 不可用时降级放行（宁可打 Meta 也不能完全阻断业务）
            _logger.LogWarning(ex, "Meta API 限流器 Redis 操作失败，降级放行");
            return (true, 0);
        }
    }

    private string BuildKey(string bucketType)
        => $"{_options.KeyPrefix}:{bucketType}";
}

/// <summary>
/// Meta API 限流异常 — 表示当前请求超过配置的速率限制
/// </summary>
public class MetaApiRateLimitException : Exception
{
    public MetaApiRateLimitException(string message) : base(message) { }
    public MetaApiRateLimitException(string message, Exception inner) : base(message, inner) { }
}
