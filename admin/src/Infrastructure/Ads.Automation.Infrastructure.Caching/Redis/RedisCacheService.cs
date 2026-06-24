namespace Ads.Automation.Infrastructure.Caching.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisCacheService> _logger;
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisCacheService(IRedisConnectionProvider provider, ILogger<RedisCacheService> logger)
        {
            _db = provider.Database ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            var value = await _db.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                _logger.LogDebug("Cache miss: {Key}", key);
                return default;
            }

            _logger.LogDebug("Cache hit: {Key}", key);

            var raw = value.ToString();
            return JsonSerializer.Deserialize<T>(raw, SerializerOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            ArgumentNullException.ThrowIfNull(key);

            var serialized = JsonSerializer.Serialize(value, SerializerOptions);

            if (expiration.HasValue)
                await _db.StringSetAsync(key, serialized, expiration.Value).ConfigureAwait(false);
            else
                await _db.StringSetAsync(key, serialized).ConfigureAwait(false);

            _logger.LogDebug("Cache set: {Key}, Expiry: {Expiry}", key, expiration);
        }

        public async Task RemoveAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            await _db.KeyDeleteAsync(key);
            _logger.LogDebug("Cache removed: {Key}", key);
        }

        public async Task RemoveAllAsync(IEnumerable<string> keys)
        {
            ArgumentNullException.ThrowIfNull(keys);

            var keyArray = keys as string[] ?? keys.ToArray();
            if (keyArray.Length == 0) return;

            var redisKeys = keyArray.Select(k => (RedisKey)k).ToArray();
            await _db.KeyDeleteAsync(redisKeys);
            _logger.LogDebug("Cache batch removed: {Count} keys", redisKeys.Length);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            ArgumentNullException.ThrowIfNull(key);

            return await _db.KeyExistsAsync(key);
        }

        public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(factory);

            var cached = await GetAsync<T>(key);
            if (cached is not null)
                return cached;

            var value = await factory();

            if (value is not null)
                await SetAsync(key, value, expiration);

            return value;
        }

        public async Task RefreshAsync(string key, TimeSpan expiration)
        {
            ArgumentNullException.ThrowIfNull(key);

            await _db.KeyExpireAsync(key, expiration);
            _logger.LogDebug("Cache refreshed: {Key}, Expiry: {Expiry}", key, expiration);
        }

        public async Task<string[]> GetKeysByPatternAsync(string pattern)
        {
            ArgumentNullException.ThrowIfNull(pattern);

            var keys = new List<string>();
            var endpoints = _db.Multiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _db.Multiplexer.GetServer(endpoint);
                if (!server.IsConnected) continue;

                await foreach (var key in server.KeysAsync(pattern: pattern).ConfigureAwait(false))
                {
                    keys.Add(key.ToString());
                }
            }

            return keys.ToArray();
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            ArgumentNullException.ThrowIfNull(pattern);

            var keys = await GetKeysByPatternAsync(pattern);
            if (keys.Length > 0)
            {
                var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
                await _db.KeyDeleteAsync(redisKeys);
                _logger.LogDebug("Cache removed by pattern: {Pattern}, {Count} keys", pattern, keys.Length);
            }
        }
    }
}
