namespace Ads.Automation.Infrastructure.RabbitMq.Interfaces;

/// <summary>
/// RabbitMQ 连接工厂接口
/// </summary>
public interface IRabbitMqConnectionFactory : IDisposable
{
    /// <summary>获取连接</summary>
    IConnection GetConnection();

    /// <summary>创建 Channel（支持发布者确认）</summary>
    IModel CreateChannel();

    /// <summary>借用 Channel（从池中获取）</summary>
    IModel LeaseChannel();

    /// <summary>归还 Channel 到池中</summary>
    void ReturnChannel(IModel channel);
}
