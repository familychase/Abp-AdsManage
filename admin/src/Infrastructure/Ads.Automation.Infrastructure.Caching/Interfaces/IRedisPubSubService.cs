namespace Ads.Automation.Infrastructure.Caching.Interfaces
{
    /// <summary>
    /// 发布订阅服务接口，提供发布消息和订阅消息的方法，允许在分布式系统中实现消息传递和事件通知功能。
    /// </summary>
    public interface IRedisPubSubService
    {
        /// <summary>
        /// 发布消息到指定频道
        /// </summary>
        Task PublishAsync<T>(string channel, T message);

        /// <summary>
        /// 订阅指定频道的消息
        /// </summary>
        Task SubscribeAsync<T>(string channel, Func<T, Task> handler);

        /// <summary>
        /// 取消订阅指定频道的所有消息
        /// </summary>
        Task UnsubscribeAsync(string channel);

        /// <summary>
        /// 取消所有订阅
        /// </summary>
        Task UnsubscribeAllAsync();
    }
}
