using Microsoft.Extensions.Hosting;

namespace Ads.Automation.Infrastructure.RabbitMq.Extensions;

/// <summary>
/// RabbitMQ 服务注册扩展方法
/// </summary>
public static class RabbitMqServiceCollectionExtensions
{
    /// <summary>
    /// 注册 RabbitMQ 基础设施服务
    /// </summary>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = RabbitMqOptions.SectionName,
        Action<RabbitMqOptions>? configureOptions = null)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(sectionName));

        if (configureOptions != null)
            services.Configure(configureOptions);

        services.Configure<RetryPolicyOptions>(configuration.GetSection("RabbitMq:RetryPolicy"));

        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
        services.AddSingleton<MessageBus>();
        services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<MessageBus>());
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<MessageBus>());

        return services;
    }

    /// <summary>
    /// 注册 RabbitMQ 基础设施服务（手动指定配置）
    /// </summary>
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> configureOptions,
        Action<RetryPolicyOptions>? configureRetry = null)
    {
        services.Configure(configureOptions);

        if (configureRetry != null)
            services.Configure(configureRetry);

        services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddSingleton<IMessageConsumer, RabbitMqConsumer>();
        services.AddSingleton<MessageBus>();
        services.AddSingleton<IMessageBus>(sp => sp.GetRequiredService<MessageBus>());
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<MessageBus>());

        return services;
    }

    /// <summary>
    /// 注册任务队列（Singleton），路由由 MessageBus 按 [MessageRoute] 特性自动推断。
    /// 调用方直接 Enqueue<typeparamref name="T"/>(job) 即可，无需区分消息类型。
    /// </summary>
    public static IServiceCollection AddJobQueue(this IServiceCollection services)
    {
        services.AddSingleton<IJobQueue, JobQueue>();
        return services;
    }

    /// <summary>
    /// 一键注册 RabbitMQ 消费者（生产者入队 + 消费者订阅），一行代码绑定消息→消费端。
    ///
    /// <code>
    /// context.Services.AddRabbitMqConsumer&lt;DuplicateTaskMessage, DuplicateJobHandler&gt;();
    /// </code>
    /// </summary>
    /// <typeparam name="TMessage">消息类型（需标注 [MessageRoute]）</typeparam>
    /// <typeparam name="TConsumer">消费者类型（继承 RabbitMqConsumerBase&lt;TMessage&gt;）</typeparam>
    public static IServiceCollection AddRabbitMqConsumer<TMessage, TConsumer>(this IServiceCollection services)
        where TConsumer : RabbitMqConsumerBase<TMessage>
    {
        services.AddSingleton<TConsumer>();
        services.AddSingleton<IHostedService>(sp => sp.GetRequiredService<TConsumer>());
        return services;
    }
}
