namespace Ads.Automation.Infrastructure.Caching.Redis
{
    public class RedisPubSubService : IRedisPubSubService
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger<RedisPubSubService> _logger;
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisPubSubService(IRedisConnectionProvider provider, ILogger<RedisPubSubService> logger)
        {
            _subscriber = provider.Multiplexer.GetSubscriber();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishAsync<T>(string channel, T message)
        {
            ArgumentNullException.ThrowIfNull(channel);

            var payload = JsonSerializer.Serialize(message, SerializerOptions);

            await _subscriber.PublishAsync(RedisChannel.Literal(channel), payload).ConfigureAwait(false);
            // _logger.LogDebug("Published to channel: {Channel}", channel);
        }

        public async Task SubscribeAsync<T>(string channel, Func<T, Task> handler)
        {
            ArgumentNullException.ThrowIfNull(channel);
            ArgumentNullException.ThrowIfNull(handler);

            await _subscriber.SubscribeAsync(RedisChannel.Literal(channel), (ch, value) =>
            {
                try
                {
                    var raw = value.ToString();
                    var msg = JsonSerializer.Deserialize<T>(raw, SerializerOptions);
                    if (msg is not null)
                    {
                        _ = handler(msg);
                    }
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "Error handling message on channel: {Channel}", channel);
                }
            }).ConfigureAwait(false);

            // _logger.LogDebug("Subscribed to channel: {Channel}", channel);
        }

        public async Task UnsubscribeAsync(string channel)
        {
            ArgumentNullException.ThrowIfNull(channel);

            await _subscriber.UnsubscribeAsync(RedisChannel.Literal(channel)).ConfigureAwait(false);
            // _logger.LogDebug("Unsubscribed from channel: {Channel}", channel);
        }

        public async Task UnsubscribeAllAsync()
        {
            await _subscriber.UnsubscribeAllAsync().ConfigureAwait(false);
            // _logger.LogDebug("Unsubscribed all channels");
        }
    }
}
