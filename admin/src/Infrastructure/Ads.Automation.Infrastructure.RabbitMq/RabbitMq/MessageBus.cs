using Microsoft.Extensions.Hosting;

namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// 消息总线实现 —— 约定路由 + 异步后台推送
/// 继承 BackgroundService，参与主机优雅关闭流程：
///   StopAsync → Writer.Complete() 停止接收新消息 → 排空 Channel 剩余消息（30s 可配）→ 退出
/// </summary>
public class MessageBus : BackgroundService, IMessageBus
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<MessageBus> _logger;
    private readonly RabbitMqOptions _options;
    private readonly System.Threading.Channels.Channel<PublishTask> _channel;

    private record PublishTask(Type MessageType, object Message, int? DelayMs, TaskCompletionSource Completion);

    public MessageBus(IMessagePublisher publisher, IOptions<RabbitMqOptions> options, ILogger<MessageBus> logger)
    {
        _publisher = publisher;
        _logger = logger;
        _options = options.Value;
        _channel = System.Threading.Channels.Channel.CreateBounded<PublishTask>(
            new System.Threading.Channels.BoundedChannelOptions(10000)
            {
                FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait
            });
    }

    /// <inheritdoc/>
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        var tcs = new TaskCompletionSource();
        await _channel.Writer.WriteAsync(
            new PublishTask(typeof(T), message, null, tcs), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task PublishWithDelayAsync<T>(T message, int delayMs, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        var tcs = new TaskCompletionSource();
        await _channel.Writer.WriteAsync(
            new PublishTask(typeof(T), message, delayMs, tcs), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>后台消息处理循环，由 IHostedService 框架自动启停</summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PublishTask task;
            try
            {
                task = await _channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (System.Threading.Channels.ChannelClosedException)
            {
                break;
            }

            await ProcessOneAsync(task).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 优雅关闭：
    /// 1. Writer.Complete() 拒绝新写入
    /// 2. 等待 ExecuteAsync 自然退出
    /// 3. 排空 Channel 中剩余消息（超时可配，默认 30s）
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _channel.Writer.Complete();

        // 先让 ExecuteAsync 自然结束（背景循环可能还在 ReadAsync 阻塞）
        await base.StopAsync(cancellationToken).ConfigureAwait(false);

        // 排空 Channel 中未被 ExecuteAsync 消费的剩余消息
        var drainTimeoutSeconds = _options.MessageBusDrainTimeoutSeconds > 0
            ? _options.MessageBusDrainTimeoutSeconds
            : 30;

        using var drainCts = new CancellationTokenSource(TimeSpan.FromSeconds(drainTimeoutSeconds));
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, drainCts.Token);

        var drained = 0;
        try
        {
            await foreach (var task in _channel.Reader.ReadAllAsync(linkedCts.Token).ConfigureAwait(false))
            {
                await ProcessOneAsync(task).ConfigureAwait(false);
                drained++;
            }
        }
        catch (OperationCanceledException)
        {
            // 超时或外部取消 —— 剩余消息无法排空
        }

        var remaining = _channel.Reader.Count;
        if (remaining > 0)
        {
            _logger.LogWarning(
                "[MessageBus] 关闭阶段未完全排空: 已处理 {Drained} 条, 剩余 {Remaining} 条将被丢弃（超时 {Timeout}s）",
                drained, remaining, drainTimeoutSeconds);
        }
        else if (drained > 0)
        {
            _logger.LogInformation("[MessageBus] 关闭排空完成: 共处理 {Drained} 条消息", drained);
        }
    }

    private async Task ProcessOneAsync(PublishTask task)
    {
        try
        {
            await SendMessageAsync(task.MessageType, task.Message, task.DelayMs).ConfigureAwait(false);
            task.Completion.TrySetResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[MessageBus] 后台发布失败: Type={MessageType}", task.MessageType.Name);
            task.Completion.TrySetException(ex);
        }
    }

    private Task SendMessageAsync(Type messageType, object message, int? delayMs)
    {
        var (exchange, routingKey) = MessageRouteResolver.ResolveRoute(messageType);

        if (delayMs.HasValue)
            return _publisher.PublishWithDelayAsync(exchange, routingKey, message, delayMs.Value);

        return _publisher.PublishAsync(exchange, routingKey, message);
    }
}
