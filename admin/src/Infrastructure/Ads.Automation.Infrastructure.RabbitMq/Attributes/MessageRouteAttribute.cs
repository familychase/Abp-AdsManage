namespace Ads.Automation.Infrastructure.RabbitMq.Attributes;

/// <summary>
/// 标记消息类的路由规则，不标记则按约定自动推断
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MessageRouteAttribute : Attribute
{
    /// <summary>交换机名称</summary>
    public string? Exchange { get; set; }

    /// <summary>路由键</summary>
    public string? RoutingKey { get; set; }
}
