namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// 消息路由解析器 —— 公共路由推断逻辑
/// </summary>
public static class MessageRouteResolver
{
    /// <summary>推断路由：优先读取 [MessageRoute]，否则按约定</summary>
    public static (string Exchange, string RoutingKey) ResolveRoute(Type messageType)
    {
        // 按名称匹配 [MessageRoute]，不限制命名空间，Contracts/Domain.Shared 均可标注
        var routeAttr = messageType.GetCustomAttributes(false)
            .FirstOrDefault(a => a.GetType().Name == "MessageRouteAttribute");

        if (routeAttr != null)
        {
            var attrType = routeAttr.GetType();
            return (
                (attrType.GetProperty("Exchange")?.GetValue(routeAttr) as string) ?? "ads.automation.default",
                (attrType.GetProperty("RoutingKey")?.GetValue(routeAttr) as string) ?? messageType.Name.ToLowerInvariant()
            );
        }

        var ns = messageType.Namespace ?? "default";
        var exchange = string.Join('.', ns.Split('.').Reverse().Take(3).Reverse());
        var routingKey = ToKebabCase(messageType.Name);

        return (exchange, routingKey);
    }

    /// <summary>PascalCase → kebab-case（仅移除尾部后缀）</summary>
    internal static string ToKebabCase(string name)
    {
        name = StripSuffix(name, "Message")
            ?? StripSuffix(name, "Event")
            ?? StripSuffix(name, "Dto")
            ?? name;

        if (string.IsNullOrEmpty(name)) return "message";

        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? $"-{char.ToLower(c)}" : char.ToLower(c).ToString()));
    }

    /// <summary>仅移除字符串尾部后缀</summary>
    private static string? StripSuffix(string name, string suffix)
    {
        if (name.Length > suffix.Length && name.EndsWith(suffix, StringComparison.Ordinal))
            return name[..^suffix.Length];
        return null;
    }
}
