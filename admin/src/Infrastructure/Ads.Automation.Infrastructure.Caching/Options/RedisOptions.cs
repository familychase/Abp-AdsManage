namespace Ads.Automation.Infrastructure.Caching.Options
{
    public class RedisOptions
    {
        public string ConnectionString { get; set; } = string.Empty;

        public int DefaultDatabase { get; set; } = -1;

        public int ConnectTimeout { get; set; } = 5000;

        public int SyncTimeout { get; set; } = 5000;

        public int ConnectRetry { get; set; } = 3;

        public bool AllowAdmin { get; set; } = false;

        public bool Ssl { get; set; } = false;

        public int PoolSize { get; set; } = 5;

        public bool AbortOnConnectFail { get; set; } = true;
    }
}
