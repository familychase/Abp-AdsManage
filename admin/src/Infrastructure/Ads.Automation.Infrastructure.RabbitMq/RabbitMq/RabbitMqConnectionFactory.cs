namespace Ads.Automation.Infrastructure.RabbitMq.RabbitMq;

/// <summary>
/// RabbitMQ 连接工厂 —— 管理连接和 Channel 池，启动时延迟连接 + 自动重试
/// </summary>
public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;
    private readonly ConcurrentBag<IModel> _channelPool;
    private readonly object _connectionLock = new();
    private IConnection? _connection;
    private int _createdChannelCount;
    private int _poolSize;
    private bool _disposed;

    public RabbitMqConnectionFactory(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionFactory> logger)
    {
        _options = options.Value;
        _logger = logger;
        _channelPool = new ConcurrentBag<IModel>();
    }

    /// <summary>带重试的连接建立，避免启动即崩溃</summary>
    private IConnection BuildConnectionWithRetry()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            VirtualHost = _options.VirtualHost,
            UserName = _options.UserName,
            Password = _options.Password,
            AutomaticRecoveryEnabled = false, // AutorecoveringModel 屏蔽了 consumer.Received 事件
            NetworkRecoveryInterval = TimeSpan.FromMilliseconds(_options.NetworkRecoveryInterval),
            RequestedHeartbeat = TimeSpan.FromSeconds(_options.RequestedHeartbeat),
            ContinuationTimeout = TimeSpan.FromMilliseconds(_options.ConnectionTimeout)
        };

        const int maxAttempts = 3;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var connection = factory.CreateConnection();
                WireConnectionEvents(connection);
                _logger.LogInformation("RabbitMQ 连接已建立: {Host}:{Port}", _options.Host, _options.Port);
                return connection;
            }
            catch (BrokerUnreachableException ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "RabbitMQ 连接失败 (第 {Attempt}/{MaxAttempts} 次)，{Delay} 秒后重试...",
                    attempt, maxAttempts, 3);
                Thread.Sleep(3000);
            }
        }

        // 最后一次尝试不再 catch，让异常上浮
        _logger.LogCritical("RabbitMQ 连接失败，已达最大重试次数 {MaxAttempts}", maxAttempts);
        try
        {
            var connection = factory.CreateConnection();
            WireConnectionEvents(connection);
            return connection;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"无法连接到 RabbitMQ ({_options.Host}:{_options.Port})，已重试 {maxAttempts} 次", ex);
        }
    }

    private void WireConnectionEvents(IConnection connection)
    {
        connection.ConnectionShutdown += (_, e) =>
            { _logger.LogWarning("RabbitMQ 连接关闭: {ReplyText}", e.ReplyText); };
        connection.ConnectionBlocked += (_, e) =>
            { _logger.LogWarning("RabbitMQ 连接阻塞: {Reason}", e.Reason); };
        connection.CallbackException += (_, ea) =>
            { _logger.LogError(ea.Exception, "RabbitMQ 回调异常"); };
    }

    /// <inheritdoc/>
    public IConnection GetConnection()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        lock (_connectionLock)
        {
            if (_connection is { IsOpen: true })
                return _connection;

            _logger.LogInformation("RabbitMQ 连接不可用，尝试重新连接...");
            try { _connection?.Dispose(); } catch { /* 忽略 */ }
            _connection = BuildConnectionWithRetry();
            return _connection;
        }
    }

    /// <inheritdoc/>
    public IModel CreateChannel()
    {
        var channel = GetConnection().CreateModel();

        if (_options.PublisherConfirms)
            channel.ConfirmSelect();

        return channel;
    }

    /// <inheritdoc/>
    public IModel LeaseChannel()
    {
        if (_channelPool.TryTake(out var channel))
        {
            if (channel.IsOpen)
            {
                Interlocked.Decrement(ref _poolSize);
                return channel;
            }

            Interlocked.Decrement(ref _poolSize);
            channel.Dispose();
        }

        var newChannel = CreateChannel();
        Interlocked.Increment(ref _createdChannelCount);
        return newChannel;
    }

    /// <inheritdoc/>
    public void ReturnChannel(IModel channel)
    {
        if (!channel.IsOpen)
        {
            channel.Dispose();
            return;
        }

        if (_poolSize >= _options.MaxChannelPoolSize)
        {
            channel.Dispose();
            return;
        }

        _channelPool.Add(channel);
        Interlocked.Increment(ref _poolSize);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        while (_channelPool.TryTake(out var channel))
        {
            try { channel.Close(); channel.Dispose(); }
            catch { /* 忽略关闭异常 */ }
        }

        if (_connection != null)
        {
            try { _connection.Close(); _connection.Dispose(); }
            catch { /* 忽略关闭异常 */ }
        }

        // _logger.LogInformation("RabbitMQ 连接已释放");
    }
}
