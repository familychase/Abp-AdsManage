namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// RabbitMQ 消息发布者 —— 可控推流
/// </summary>
public class RabbitMqPublisher : IMessagePublisher, IDisposable
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly RabbitMqOptions _options;
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    private readonly ManualResetEventSlim _pauseEvent = new(true);
    private int _paused;

    public RabbitMqPublisher(IRabbitMqConnectionFactory connectionFactory, IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public bool IsPaused => _paused == 1;

    /// <inheritdoc/>
    public void Pause()
    {
        if (Interlocked.Exchange(ref _paused, 1) == 0)
        {
            _pauseEvent.Reset();
            // _logger.LogInformation("消息推流已暂停");
        }
    }

    /// <inheritdoc/>
    public void Resume()
    {
        if (Interlocked.Exchange(ref _paused, 0) == 1)
        {
            _pauseEvent.Set();
            // _logger.LogInformation("消息推流已恢复");
        }
    }

    /// <inheritdoc/>
    public Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default)
    {
        return PublishAsync(exchange, routingKey, message, new PublishOptions(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task PublishAsync<T>(string exchange, string routingKey, T message, PublishOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);
        ArgumentNullException.ThrowIfNull(routingKey);
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(options);

        // 可控推流：等待恢复信号
        _pauseEvent.Wait(cancellationToken);

        var channel = _connectionFactory.LeaseChannel();
        try
        {
            await PublishInternalAsync(channel, exchange, routingKey, message, options, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <inheritdoc/>
    public async Task PublishBatchAsync<T>(string exchange, string routingKey, IEnumerable<T> messages, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);
        ArgumentNullException.ThrowIfNull(routingKey);
        ArgumentNullException.ThrowIfNull(messages);

        var messageList = messages.ToList();
        if (messageList.Count == 0) return;

        // 可控推流：等待恢复信号
        _pauseEvent.Wait(cancellationToken);

        var channel = _connectionFactory.LeaseChannel();
        try
        {
            var options = new PublishOptions();
            var remaining = new Queue<T>(messageList);

            while (remaining.Count > 0)
            {
                var batch = new List<T>();
                for (int i = 0; i < _options.MaxBatchSize && remaining.Count > 0; i++)
                    batch.Add(remaining.Dequeue());

                // 串行发送批次内的每条消息，避免同一 Channel 上并发 WaitForConfirms 确认错乱
                foreach (var msg in batch)
                {
                    await PublishInternalAsync(channel, exchange, routingKey, msg, options, cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            // _logger.LogDebug("批量发布完成: Exchange={Exchange}, Count={Count}", exchange, messageList.Count);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <inheritdoc/>
    public async Task PublishWithDelayAsync<T>(string exchange, string routingKey, T message, int delayMs, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(exchange);
        ArgumentNullException.ThrowIfNull(routingKey);
        ArgumentNullException.ThrowIfNull(message);

        _pauseEvent.Wait(cancellationToken);

        // 延迟消息通过 TTL + DLX 方式实现
        // 创建延时交换机
        var delayExchange = $"{exchange}.delay.{delayMs}ms";

        var channel = _connectionFactory.LeaseChannel();
        try
        {
            // 声明延时交换机和延时队列
            channel.ExchangeDeclare(delayExchange, "x-delayed-message", durable: true, autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-delayed-type", "topic" }
                });

            var options = new PublishOptions
            {
                Headers = new Dictionary<string, object?>
                {
                    { "x-delay", delayMs }
                }
            };

            await PublishInternalAsync(channel, delayExchange, routingKey, message, options, cancellationToken)
                .ConfigureAwait(false);

            // _logger.LogDebug("延迟消息发布: Exchange={Exchange}, RoutingKey={RoutingKey}, Delay={DelayMs}ms",
            //     exchange, routingKey, delayMs);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <summary>内部发布实现</summary>
    private async Task PublishInternalAsync<T>(IModel channel, string exchange, string routingKey, T message, PublishOptions options, CancellationToken cancellationToken)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message, SerializerOptions));
        var props = options.CreateProperties(channel);

        // 设置默认持久化
        if (!options.Persistent.HasValue)
            props.Persistent = _options.Persistent;

        // 设置消息 ID
        if (string.IsNullOrEmpty(props.MessageId))
            props.MessageId = Guid.NewGuid().ToString("N");

        // 声明交换机（幂等，确保 Consumer 未启动时 Exchange 也存在）
        channel.ExchangeDeclare(exchange, "direct", durable: true, autoDelete: false);

        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: props,
            body: body);

        // 等待发布者确认（IModel 无异步版本，通过 Task.Run 包装避免阻塞异步上下文）
        if (_options.PublisherConfirms)
        {
            var confirmed = await Task.Run(() =>
                    channel.WaitForConfirms(TimeSpan.FromSeconds(5), out _),
                cancellationToken).ConfigureAwait(false);

            if (!confirmed)
            {
                throw new InvalidOperationException(
                    $"消息发布未在 5s 内确认: Exchange={exchange}, RoutingKey={routingKey}");
            }
        }

        _logger.LogInformation(
            "消息已发布: Type={MessageType}, Exchange={Exchange}, RoutingKey={RoutingKey}",
            typeof(T).Name, exchange, routingKey);
    }

    public void Dispose()
    {
        _pauseEvent.Dispose();
    }
}
