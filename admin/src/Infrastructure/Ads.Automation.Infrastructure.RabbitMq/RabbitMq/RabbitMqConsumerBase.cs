using Microsoft.Extensions.Hosting;

namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// RabbitMQ 消费端基类（事件驱动，非定时轮询） —— 一行继承即可完成 Exchange 声明、Queue 绑定、订阅、保活。
///
/// 用法：
/// <code>
/// public class MyConsumer : RabbitMqConsumerBase&lt;MyMessage&gt;
/// {
///     public MyConsumer(IMessageConsumer consumer, ILogger&lt;MyConsumer&gt; logger) : base(consumer, logger) { }
///     protected override Task&lt;bool&gt; HandleAsync(MyMessage msg, CancellationToken ct) { ... }
/// }
/// </code>
/// 然后在 Module 中 AddHostedService&lt;MyConsumer&gt;() 即可。
/// </summary>
/// <typeparam name="TMessage">消息类型（需标注 [MessageRoute] 指定 Exchange/RoutingKey）</typeparam>
public abstract class RabbitMqConsumerBase<TMessage> : IHostedService, IDisposable
{
    private readonly IMessageConsumer _consumer;
    private readonly ILogger _logger;
    private readonly (string Exchange, string RoutingKey) _route;
    private CancellationTokenSource? _cts;
    private Task? _subscriptionTask;
    private bool _disposed;

    /// <summary>队列名称，默认取 RoutingKey 同名</summary>
    protected virtual string Queue => _route.RoutingKey;

    /// <summary>交换机类型</summary>
    protected virtual string ExchangeType => "direct";

    /// <summary>是否自动确认（默认手动 Ack，可靠性优先）</summary>
    protected virtual bool AutoAck => false;

    /// <summary>交换机名称（自动从 [MessageRoute] 推断）</summary>
    protected string Exchange => _route.Exchange;

    /// <summary>路由键（自动从 [MessageRoute] 推断）</summary>
    protected string RoutingKey => _route.RoutingKey;

    protected RabbitMqConsumerBase(IMessageConsumer consumer, ILogger logger)
    {
        _consumer = consumer;
        _logger = logger;
        _route = MessageRouteResolver.ResolveRoute(typeof(TMessage));
    }

    /// <summary>子类只需覆写业务逻辑，返回 true=Ack / false=Nack+重试</summary>
    protected abstract Task<bool> HandleAsync(TMessage message, CancellationToken cancellationToken);

    /// <summary>启动消费者（IHostedService）</summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{Consumer}] 正在启动，目标 Exchange={Exchange} Queue={Queue} RoutingKey={RoutingKey}",
            GetType().Name, _route.Exchange, Queue, _route.RoutingKey);

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _subscriptionTask = Task.Run(async () =>
        {
            await Task.Yield();

            try
            {
                _logger.LogInformation("[{Consumer}] 正在启动，准备订阅 Exchange={Exchange} Queue={Queue}",
                    GetType().Name, _route.Exchange, Queue);

                // 1. 声明交换机（幂等）
                _consumer.ExchangeDeclare(_route.Exchange, ExchangeType, durable: true);

                _logger.LogInformation("[{Consumer}] 交换机已声明: {Exchange}",
                    GetType().Name, _route.Exchange);

                // 2. 订阅（内部自动：声明队列 + 绑定到交换机 + 开始消费）
                await _consumer.SubscribeAsync<TMessage>(
                    queue: Queue,
                    exchange: _route.Exchange,
                    routingKey: _route.RoutingKey,
                    handler: HandleAsync,
                    autoAck: AutoAck,
                    cancellationToken: _cts.Token);

                _logger.LogInformation(
                    "[{Consumer}] 已就绪 Queue={Queue} Exchange={Exchange} RK={RoutingKey}",
                    GetType().Name, Queue, _route.Exchange, _route.RoutingKey);

                // 3. 保活直到关闭
                var tcs = new TaskCompletionSource();
                _cts.Token.Register(() => tcs.TrySetResult());
                await tcs.Task;
            }
            catch (OperationCanceledException) when (_cts?.IsCancellationRequested == true)
            {
                _logger.LogInformation("[{Consumer}] 正常关闭", GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "[{Consumer}] 致命错误，RabbitMQ 消费者启动失败: {ErrorMessage}", GetType().Name, ex.Message);
            }
        }, _cts.Token);

        return Task.CompletedTask;
    }

    /// <summary>停止消费者（IHostedService）</summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[{Consumer}] 正在停止...", GetType().Name);
        _cts?.Cancel();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
