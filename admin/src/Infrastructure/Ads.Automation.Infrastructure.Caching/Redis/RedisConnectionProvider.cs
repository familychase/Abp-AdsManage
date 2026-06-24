using Microsoft.Extensions.Options;

namespace Ads.Automation.Infrastructure.Caching.Redis
{
    public class RedisConnectionProvider : IRedisConnectionProvider, IDisposable
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private bool _disposed;

        public RedisConnectionProvider(IOptions<RedisOptions> options)
        {
            var connectionString = options.Value.ConnectionString;
            var configOptions = ConfigurationOptions.Parse(connectionString);

            if (options.Value.DefaultDatabase >= 0)
                configOptions.DefaultDatabase = options.Value.DefaultDatabase;

            configOptions.ConnectTimeout = options.Value.ConnectTimeout;
            configOptions.SyncTimeout = options.Value.SyncTimeout;
            configOptions.ConnectRetry = options.Value.ConnectRetry;
            configOptions.AllowAdmin = options.Value.AllowAdmin;
            configOptions.Ssl = options.Value.Ssl;
            // 强制 AbortOnConnectFail=true，确保连接失败直接抛异常，程序不启动
            configOptions.AbortOnConnectFail = true;

            try
            {
                _redis = ConnectionMultiplexer.Connect(configOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Redis 连接失败，程序无法启动: {ex.Message}", ex);
            }

            // 二次验证：实际 PING 测试连接可用性
            try
            {
                var pingResult = _redis.GetDatabase().Ping();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Redis PING 失败，程序无法启动: {ex.Message}", ex);
            }

            if (!_redis.IsConnected)
            {
                throw new InvalidOperationException("Redis 连接未建立，程序无法启动");
            }

            _database = _redis.GetDatabase();
        }

        public IDatabase Database => _database;

        public ConnectionMultiplexer Multiplexer => _redis;

        public bool IsConnected => _redis.IsConnected;

        public void Dispose()
        {
            if (!_disposed)
            {
                _redis?.Dispose();
                _disposed = true;
            }
        }
    }
}
