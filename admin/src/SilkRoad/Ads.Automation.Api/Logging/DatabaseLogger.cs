using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Ads.Automation.Api.Logging;

/// <summary>
/// 将日志写入数据库的 ILogger 实现
/// 使用后台批量写入机制，避免阻塞请求线程
/// </summary>
public class DatabaseLogger : ILogger
{
    private readonly string _categoryName;
    private readonly DatabaseLoggerProcessor _processor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DatabaseLogger(
        string categoryName,
        DatabaseLoggerProcessor processor,
        IHttpContextAccessor httpContextAccessor)
    {
        _categoryName = categoryName;
        _processor = processor;
        _httpContextAccessor = httpContextAccessor;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        var requestPath = _httpContextAccessor.HttpContext?.Request.Path.ToString();

        _processor.Enqueue(new LogEntry
        {
            Level = logLevel.ToString(),
            Logger = _categoryName,
            Message = message,
            Exception = exception?.ToString(),
            RequestPath = requestPath
        });
    }
}
