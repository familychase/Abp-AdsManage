using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Ads.Automation.Api.Logging;

/// <summary>
/// 数据库日志提供程序
/// </summary>
public class DatabaseLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private readonly DatabaseLoggerProcessor _processor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ConcurrentDictionary<string, DatabaseLogger> _loggers = new();
    private IExternalScopeProvider? _scopeProvider;

    public DatabaseLoggerProvider(
        DatabaseLoggerProcessor processor,
        IHttpContextAccessor httpContextAccessor)
    {
        _processor = processor;
        _httpContextAccessor = httpContextAccessor;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name =>
            new DatabaseLogger(name, _processor, _httpContextAccessor));
    }

    void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
}
