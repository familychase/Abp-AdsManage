

namespace Ads.Automation.Infrastructure.Caching.Interfaces
{
    public interface IRedisConnectionProvider
    {
        IDatabase Database { get; }

        ConnectionMultiplexer Multiplexer { get; }

        bool IsConnected { get; }
    }
}
