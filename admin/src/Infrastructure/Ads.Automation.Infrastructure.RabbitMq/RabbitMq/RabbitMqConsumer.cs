namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// RabbitMQ 消息消费者 —— 可靠性重推
/// </summary>
public class RabbitMqConsumer : IMessageConsumer
{
    private readonly IRabbitMqConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly RabbitMqOptions _options;
    private readonly RetryPolicyOptions _retryOptions;
    private readonly ConcurrentDictionary<string, IModel> _activeChannels = new();
    private readonly ConcurrentDictionary<string, IBasicConsumer> _activeConsumers = new();
    private readonly ConcurrentDictionary<string, SubscriptionInfo> _subscriptions = new();
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string RetryCountHeader = "x-retry-count";
    private bool _disposed;

    private sealed record SubscriptionInfo
    {
        public Type MessageType { get; init; } = null!;
        public string Queue { get; init; } = null!;
        public string Exchange { get; init; } = null!;
        public string RoutingKey { get; init; } = null!;
        public Delegate Handler { get; init; } = null!;
        public bool AutoAck { get; init; }
    }

    public RabbitMqConsumer(IRabbitMqConnectionFactory connectionFactory, IOptions<RabbitMqOptions> options, IOptions<RetryPolicyOptions> retryOptions, ILogger<RabbitMqConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _options = options.Value;
        _retryOptions = retryOptions.Value;
    }

    /// <inheritdoc/>
    public async Task<string> SubscribeAsync<T>(string queue, string exchange, string routingKey, Func<T, CancellationToken, Task<bool>> handler, bool autoAck = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(exchange);
        ArgumentNullException.ThrowIfNull(routingKey);
        ArgumentNullException.ThrowIfNull(handler);

        // 确保使用死信配置的队列存在（含 DLX/DLQ/retry 完整拓扑）
        EnsureQueueWithDlx(queue);

        var channel = _connectionFactory.GetConnection().CreateModel();

        // 监听 Channel 关闭事件 — 触发自动重连
        channel.ModelShutdown += (_, args) =>
        {
            _logger.LogWarning("Consumer Channel 关闭: Queue={Queue}, Reason={Reason}, Initiator={Initiator}",
                queue, args.ReplyText, args.Initiator);

            // 非主动取消 → 自动重连
            if (args.Initiator == ShutdownInitiator.Application)
                return;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(2000); // 等待连接恢复
                    await ReconnectConsumerAsync(queue, channel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Consumer 重连失败: Queue={Queue}", queue);
                }
            });
        };

        // 绑定队列到交换机（在队列声明之后，确保队列已存在）
        channel.QueueBind(queue, exchange, routingKey);

        channel.BasicQos(0, _options.PrefetchCount, false);

        // 使用同步 EventingBasicConsumer（避免 AsyncEventingBasicConsumer + AutorecoveringModel 兼容问题）
        var consumer = new EventingBasicConsumer(channel);
        var consumerTag = channel.BasicConsume(queue: queue, autoAck: autoAck, consumer: consumer);

        consumer.Received += (_, ea) =>
        {
            _ = Task.Run(async () =>
            {
            _logger.LogInformation("收到消息: Queue={Queue}, DeliveryTag={DeliveryTag}", queue, ea.DeliveryTag);

            var deliveryTag = ea.DeliveryTag;
            var retryCount = GetRetryCount(ea.BasicProperties);

            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                T? message;

                try
                {
                    message = JsonSerializer.Deserialize<T>(body, SerializerOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "消息反序列化失败: Queue={Queue}, DeliveryTag={DeliveryTag}", queue, deliveryTag);
                    TryNack(channel, deliveryTag, autoAck, queue);
                    await SendToDeadLetterAsync(queue, ea.Body, ea.BasicProperties, retryCount);
                    return;
                }

                if (message == null)
                {
                    // _logger.LogWarning("反序列化结果为空: Queue={Queue}, DeliveryTag={DeliveryTag}", queue, deliveryTag);
                    TryNack(channel, deliveryTag, autoAck, queue);
                    return;
                }

                var success = await handler(message, cancellationToken).ConfigureAwait(false);

                if (success)
                {
                    TryAck(channel, deliveryTag, autoAck, queue);
                    _logger.LogInformation("消息已处理: Queue={Queue}, DeliveryTag={DeliveryTag}, RetryCount={RetryCount}",
                        queue, deliveryTag, retryCount);
                }
                else
                {
                    // 处理失败，进行重推
                    await HandleRetryAsync(channel, queue, deliveryTag, ea.Body, ea.BasicProperties, retryCount, autoAck);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "消息处理异常: Queue={Queue}, DeliveryTag={DeliveryTag}", queue, deliveryTag);
                TryNack(channel, deliveryTag, autoAck, queue);
                await SendToDeadLetterAsync(queue, ea.Body, ea.BasicProperties, retryCount);
            }
            });
        };

        _activeChannels[consumerTag] = channel;
        _activeConsumers[consumerTag] = consumer;

        // 存储订阅信息，用于连接断开后自动重连
        _subscriptions[consumerTag] = new SubscriptionInfo
        {
            MessageType = typeof(T),
            Queue = queue,
            Exchange = exchange,
            RoutingKey = routingKey,
            Handler = handler,
            AutoAck = autoAck
        };

        _logger.LogInformation("已订阅队列: Queue={Queue}, ConsumerTag={ConsumerTag}, AutoAck={AutoAck}",
            queue, consumerTag, autoAck);

        return consumerTag;
    }

    /// <inheritdoc/>
    public Task UnsubscribeAsync(string consumerTag)
    {
        _subscriptions.TryRemove(consumerTag, out _);

        if (_activeChannels.TryRemove(consumerTag, out var channel))
        {
            channel.BasicCancel(consumerTag);
            channel.Close();
            channel.Dispose();
            _activeConsumers.TryRemove(consumerTag, out _);
            // _logger.LogInformation("已取消订阅: ConsumerTag={ConsumerTag}", consumerTag);
        }
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Acknowledge(ulong deliveryTag, bool multiple = false)
    {
        foreach (var channel in _activeChannels.Values)
        {
            if (channel.IsOpen)
            {
                channel.BasicAck(deliveryTag, multiple);
                break;
            }
        }
    }

    /// <inheritdoc/>
    public void Reject(ulong deliveryTag, bool requeue = false)
    {
        foreach (var channel in _activeChannels.Values)
        {
            if (channel.IsOpen)
            {
                channel.BasicReject(deliveryTag, requeue);
                break;
            }
        }
    }

    /// <inheritdoc/>
    public void Nack(ulong deliveryTag, bool multiple = false, bool requeue = false)
    {
        foreach (var channel in _activeChannels.Values)
        {
            if (channel.IsOpen)
            {
                channel.BasicNack(deliveryTag, multiple, requeue);
                break;
            }
        }
    }

    /// <inheritdoc/>
    public QueueDeclareOk QueueDeclare(string queue, bool durable = true, bool exclusive = false, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
    {
        var channel = _connectionFactory.LeaseChannel();
        try
        {
            return channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <inheritdoc/>
    public void ExchangeDeclare(string exchange, string type, bool durable = true, bool autoDelete = false, IDictionary<string, object?>? arguments = null)
    {
        var channel = _connectionFactory.LeaseChannel();
        try
        {
            channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <inheritdoc/>
    public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object?>? arguments = null)
    {
        var channel = _connectionFactory.LeaseChannel();
        try
        {
            channel.QueueBind(queue, exchange, routingKey, arguments);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <inheritdoc/>
    public uint GetMessageCount(string queue)
    {
        var channel = _connectionFactory.LeaseChannel();
        try
        {
            return channel.MessageCount(queue);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    #region 连接恢复与容错

    /// <summary>安全 ACK：通道已关闭时静默忽略（消息未确认会被 RabbitMQ 重新投递）</summary>
    private void TryAck(IModel channel, ulong deliveryTag, bool autoAck, string queue)
    {
        if (autoAck) return;
        try
        {
            channel.BasicAck(deliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ACK 失败（连接已断开，消息将重投）: Queue={Queue}, DeliveryTag={DeliveryTag}",
                queue, deliveryTag);
        }
    }

    /// <summary>安全 NACK：通道已关闭时静默忽略</summary>
    private void TryNack(IModel channel, ulong deliveryTag, bool autoAck, string queue)
    {
        if (autoAck) return;
        try
        {
            channel.BasicNack(deliveryTag, false, false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "NACK 失败（连接已断开）: Queue={Queue}, DeliveryTag={DeliveryTag}",
                queue, deliveryTag);
        }
    }

    /// <summary>消费者自动重连：清理旧通道 → 重新建立连接和订阅</summary>
    private async Task ReconnectConsumerAsync(string queue, IModel oldChannel)
    {
        // 找到所有使用此队列的订阅
        var subscriptions = _subscriptions
            .Where(kv => kv.Value.Queue == queue)
            .ToList();

        if (subscriptions.Count == 0)
        {
            _logger.LogWarning("未找到队列 {Queue} 的订阅信息，跳过重连", queue);
            return;
        }

        // 清理旧通道
        foreach (var (tag, info) in subscriptions)
        {
            _activeChannels.TryRemove(tag, out _);
            _activeConsumers.TryRemove(tag, out _);
        }

        try { oldChannel.Close(); oldChannel.Dispose(); } catch { /* 忽略 */ }

        // 重新订阅
        foreach (var (_, info) in subscriptions)
        {
            try
            {
                // 使用反射调用泛型 SubscribeAsync
                var method = typeof(RabbitMqConsumer)
                    .GetMethod(nameof(SubscribeAsync), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(info.MessageType);

                var task = (Task<string>)method.Invoke(this,
                    [info.Queue, info.Exchange, info.RoutingKey, info.Handler, info.AutoAck, CancellationToken.None])!;

                var newTag = await task;
                _logger.LogInformation("Consumer 重连成功: Queue={Queue}, NewTag={NewTag}", queue, newTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer 重连失败: Queue={Queue}, Exchange={Exchange}",
                    info.Queue, info.Exchange);
            }
        }
    }

    #endregion

    #region 可靠性重推机制

    /// <summary>处理重试逻辑</summary>
    private async Task HandleRetryAsync(IModel channel, string queue, ulong deliveryTag, ReadOnlyMemory<byte> body, IBasicProperties originalProps, int currentRetryCount, bool autoAck)
    {
        var maxRetryCount = _retryOptions.MaxRetryCount;

        // 计算下次重试延迟（指数退避）
        var nextDelayMs = (int)Math.Min(
            _retryOptions.InitialRetryDelayMs * Math.Pow(_retryOptions.BackoffMultiplier, currentRetryCount),
            _retryOptions.MaxRetryDelayMs);

        if (currentRetryCount < maxRetryCount)
        {
            // 还有重试次数：Nack 不重新入队 → 进入 DLX → 延时后回到原队列
            TryNack(channel, deliveryTag, autoAck, queue);

            // _logger.LogWarning("消息处理失败，将重试: Queue={Queue}, DeliveryTag={DeliveryTag}, RetryCount={CurrentRetryCount}/{MaxRetryCount}, NextDelay={DelayMs}ms",
            //     queue, deliveryTag, currentRetryCount, maxRetryCount, nextDelayMs);

            // 更新重试计数头并发布到重试队列
            await RepublishToRetryQueueAsync(queue, body, originalProps, currentRetryCount, nextDelayMs);
        }
        else
        {
            // 超出最大重试次数：进入死信队列
            TryAck(channel, deliveryTag, autoAck, queue);

            // _logger.LogError("消息已达最大重试次数，移入死信队列: Queue={Queue}, DeliveryTag={DeliveryTag}, MaxRetryCount={MaxRetryCount}",
            //     queue, deliveryTag, maxRetryCount);

            await SendToDeadLetterAsync(queue, body, originalProps, currentRetryCount);
        }
    }

    /// <summary>确保队列配置了死信交换机</summary>
    private void EnsureQueueWithDlx(string queue)
    {
        var channel = _connectionFactory.LeaseChannel();
        try
        {
            // 声明死信交换机
            channel.ExchangeDeclare(_retryOptions.DeadLetterExchange, "direct", durable: true);

            // 声明死信队列
            channel.QueueDeclare(_retryOptions.DeadLetterQueue, durable: true, exclusive: false, autoDelete: false);
            channel.QueueBind(_retryOptions.DeadLetterQueue, _retryOptions.DeadLetterExchange, _retryOptions.DeadLetterQueue);

            // 声明重试交换机（用于延时重试）
            var retryExchange = $"{queue}.retry";
            var retryQueue = $"{queue}.retry";
            channel.ExchangeDeclare(retryExchange, "direct", durable: true);

            // 重试队列：TTL 到期后回到原队列（由每条消息的 Expiration 控制延迟，支持指数退避）
            var retryQueueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", "" },  // 默认交换机
                { "x-dead-letter-routing-key", queue }  // 路由回原队列
            };
            channel.QueueDeclare(retryQueue, durable: true, exclusive: false, autoDelete: false, retryQueueArgs);
            channel.QueueBind(retryQueue, retryExchange, retryQueue);

            // 声明主队列（带 DLX 配置）
            var queueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", retryExchange },
                { "x-dead-letter-routing-key", retryQueue }
            };
            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, queueArgs);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }
    }

    /// <summary>重新发布到重试队列</summary>
    private Task RepublishToRetryQueueAsync(string queue, ReadOnlyMemory<byte> body, IBasicProperties originalProps, int currentRetryCount, int delayMs)
    {
        var retryExchange = $"{queue}.retry";
        var retryQueue = $"{queue}.retry";

        var channel = _connectionFactory.LeaseChannel();
        try
        {
            var newProps = channel.CreateBasicProperties();
            newProps.Persistent = originalProps.Persistent;
            newProps.MessageId = originalProps.MessageId;
            newProps.CorrelationId = originalProps.CorrelationId;

            // 增加重试计数
            newProps.Headers = new Dictionary<string, object?>
            {
                { RetryCountHeader, currentRetryCount + 1 }
            };

            // 复制原始头
            if (originalProps.Headers != null)
            {
                foreach (var (key, value) in originalProps.Headers)
                {
                    if (key != RetryCountHeader)
                        newProps.Headers[key] = value;
                }
            }

            // 设置 TTL
            newProps.Expiration = delayMs.ToString();

            channel.BasicPublish(retryExchange, retryQueue, false, newProps, body);

            // _logger.LogDebug("消息已发布到重试队列: RetryQueue={RetryQueue}, RetryCount={RetryCount}, DelayMs={DelayMs}",
            //     retryQueue, currentRetryCount + 1, delayMs);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }

        return Task.CompletedTask;
    }

    /// <summary>将消息发送到死信队列</summary>
    private Task SendToDeadLetterAsync(string originalQueue, ReadOnlyMemory<byte> body, IBasicProperties originalProps, int retryCount)
    {
        var channel = _connectionFactory.LeaseChannel();
        try
        {
            var props = channel.CreateBasicProperties();
            props.Persistent = originalProps.Persistent;

            // 记录最终信息
            props.Headers = new Dictionary<string, object?>
            {
                { RetryCountHeader, retryCount },
                { "x-original-queue", originalQueue },
                { "x-failure-time", DateTimeOffset.UtcNow.ToString("O") }
            };

            if (originalProps.Headers != null)
            {
                foreach (var (key, value) in originalProps.Headers)
                {
                    if (key != RetryCountHeader)
                        props.Headers[key] = value;
                }
            }

            if (!string.IsNullOrEmpty(originalProps.MessageId))
                props.MessageId = originalProps.MessageId;

            if (!string.IsNullOrEmpty(originalProps.CorrelationId))
                props.CorrelationId = originalProps.CorrelationId;

            channel.BasicPublish(
                _retryOptions.DeadLetterExchange,
                _retryOptions.DeadLetterQueue,
                false,
                props,
                body);

            // _logger.LogError("消息已移入死信队列: OriginalQueue={OriginalQueue}, DLQ={DLQ}, RetryCount={RetryCount}",
            //     originalQueue, _retryOptions.DeadLetterQueue, retryCount);
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "移入死信队列失败: OriginalQueue={OriginalQueue}", originalQueue);
        }
        finally
        {
            _connectionFactory.ReturnChannel(channel);
        }

        return Task.CompletedTask;
    }

    /// <summary>获取消息重试次数</summary>
    private static int GetRetryCount(IBasicProperties properties)
    {
        if (properties?.Headers != null &&
            properties.Headers.TryGetValue(RetryCountHeader, out var value) &&
            value is int count)
        {
            return count;
        }
        return 0;
    }

    #endregion

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        foreach (var (tag, channel) in _activeChannels)
        {
            try
            {
                if (channel.IsOpen)
                    channel.BasicCancel(tag);
                channel.Close();
                channel.Dispose();
            }
            catch { /* 忽略关闭异常 */ }
        }

        _activeChannels.Clear();
        _activeConsumers.Clear();
        _subscriptions.Clear();

        // _logger.LogInformation("RabbitMQ 消费者已释放");
    }
}
