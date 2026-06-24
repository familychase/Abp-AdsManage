using Ads.Automation.Domain.Log;
using System.Collections.Concurrent;

namespace Ads.Automation.Api.Logging;

/// <summary>
/// 后台日志处理器：根据日志级别分别批量写入三张表
///   Information/Debug/Trace → sys_log_info
///   Warning                 → sys_log_warning
///   Error/Critical          → sys_log_error
/// </summary>
public class DatabaseLoggerProcessor : IDisposable
{
    private readonly BlockingCollection<LogEntry> _queue = new(boundedCapacity: 10000);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _backgroundTask;
    private const int BatchSize = 50;
    private const int FlushIntervalMs = 2000;

    public DatabaseLoggerProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _backgroundTask = Task.Run(ProcessQueueAsync);
    }

    public void Enqueue(LogEntry entry)
    {
        if (!_queue.IsAddingCompleted)
        {
            _queue.TryAdd(entry);
        }
    }

    private async Task ProcessQueueAsync()
    {
        var batch = new List<LogEntry>(BatchSize);

        while (!_cts.Token.IsCancellationRequested)
        {
            batch.Clear();

            try
            {
                if (_queue.TryTake(out var entry, FlushIntervalMs, _cts.Token))
                {
                    batch.Add(entry);
                }

                while (batch.Count < BatchSize && _queue.TryTake(out entry))
                {
                    batch.Add(entry);
                }

                if (batch.Count > 0)
                {
                    await SaveBatchAsync(batch);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine($"[DatabaseLogger] 批量写入日志失败: {ex.Message}");
                }
                catch { }
            }
        }

        await DrainQueueAsync();
    }

    private async Task SaveBatchAsync(List<LogEntry> batch)
    {
        using var scope = _scopeFactory.CreateScope();
        var infoRepo = scope.ServiceProvider.GetRequiredService<ISysLogInfoRepository>();
        var warnRepo = scope.ServiceProvider.GetRequiredService<ISysLogWarningRepository>();
        var errorRepo = scope.ServiceProvider.GetRequiredService<ISysLogErrorRepository>();

        var infoList = new List<SysLogInfo>();
        var warnList = new List<SysLogWarning>();
        var errorList = new List<SysLogError>();

        foreach (var entry in batch)
        {
            var level = Truncate(entry.Level, 32);
            var logger = Truncate(entry.Logger, 512);
            var message = Truncate(entry.Message, 4000);
            var exception = Truncate(entry.Exception, 8000);
            var requestPath = Truncate(entry.RequestPath, 1024);

            switch (entry.Level)
            {
                case "Information":
                case "Debug":
                case "Trace":
                    infoList.Add(SysLogInfo.Create(
                        Ads.Automation.Infrastructure.Yitter.IdGenerator.GetNextId(),
                        level, logger, message, requestPath));
                    break;

                case "Warning":
                    warnList.Add(SysLogWarning.Create(
                        Ads.Automation.Infrastructure.Yitter.IdGenerator.GetNextId(),
                        level, logger, message, exception, requestPath));
                    break;

                case "Error":
                case "Critical":
                    errorList.Add(SysLogError.Create(
                        Ads.Automation.Infrastructure.Yitter.IdGenerator.GetNextId(),
                        level, logger, message, exception, requestPath));
                    break;
            }
        }

        if (infoList.Count > 0)
            await infoRepo.InsertManyAsync(infoList, autoSave: true);
        if (warnList.Count > 0)
            await warnRepo.InsertManyAsync(warnList, autoSave: true);
        if (errorList.Count > 0)
            await errorRepo.InsertManyAsync(errorList, autoSave: true);
    }

    private async Task DrainQueueAsync()
    {
        var remaining = new List<LogEntry>();
        while (_queue.TryTake(out var entry))
        {
            remaining.Add(entry);
        }

        if (remaining.Count > 0)
        {
            try
            {
                await SaveBatchAsync(remaining);
            }
            catch { }
        }
    }

    /// <summary>
    /// 截断字符串到指定长度，避免写入数据库时超出列限制（SQL Error 8152）
    /// </summary>
    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return value.Length <= maxLength ? value : value[..maxLength];
    }

    public void Dispose()
    {
        _queue.CompleteAdding();
        _cts.Cancel();
        try { _backgroundTask.Wait(TimeSpan.FromSeconds(10)); } catch { }
        _cts.Dispose();
        _queue.Dispose();
    }
}
