using System.Collections.Concurrent;

namespace Ads.Automation.Infrastructure.Caching.Redis
{
    public class RedisDistributedLock : IDistributedLock
    {
        private readonly IDatabase _db;
        private readonly IRedisConnectionProvider _provider;
        private readonly ILogger<RedisDistributedLock> _logger;
        private readonly ConcurrentDictionary<string, string> _tokens = new();

        public RedisDistributedLock(IRedisConnectionProvider provider, ILogger<RedisDistributedLock> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _db = provider.Database ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> AcquireAsync(string key, TimeSpan expiration)
        {
            ArgumentNullException.ThrowIfNull(key);

            var token = Guid.NewGuid().ToString("N");

            try
            {
                var acquired = await _db.LockTakeAsync(key, token, expiration).ConfigureAwait(false);

                if (acquired)
                {
                    _tokens[key] = token;
                    _logger.LogDebug("Lock acquired: {Key}, Token: {Token}, Expiry: {Expiry}", key, token, expiration);
                }
                else
                {
                    _logger.LogDebug("Lock acquire failed (锁已被占用): {Key}", key);
                }

                return acquired;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "分布式锁操作异常: {Key}", key);
                return false;
            }
        }

        public async Task ReleaseAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            try
            {
                if (_tokens.TryRemove(key, out var token))
                {
                    await _db.LockReleaseAsync(key, token).ConfigureAwait(false);
                    _logger.LogDebug("Lock released: {Key}", key);
                }
                else
                {
                    _logger.LogWarning("Lock release without token: {Key}, performing best-effort delete", key);
                    await _db.KeyDeleteAsync(key).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis 不可用，分布式锁释放失败: {Key}", key);
            }
        }

        public async Task<IDisposable?> AcquireWithHandleAsync(string key, TimeSpan expiration)
        {
            var acquired = await AcquireAsync(key, expiration);
            if (!acquired)
                return null;

            return new LockHandle(this, key);
        }

        private sealed class LockHandle : IDisposable
        {
            private readonly RedisDistributedLock _owner;
            private readonly string _key;
            private bool _disposed;

            public LockHandle(RedisDistributedLock owner, string key)
            {
                _owner = owner;
                _key = key;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _owner.ReleaseAsync(_key).ConfigureAwait(false).GetAwaiter().GetResult();
                    _disposed = true;
                }
            }
        }
    }
}
