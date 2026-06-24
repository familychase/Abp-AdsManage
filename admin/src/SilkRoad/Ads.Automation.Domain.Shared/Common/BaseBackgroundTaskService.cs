using System.Globalization;
using Microsoft.Extensions.Logging;

namespace Ads.Automation.Domain.Shared.Common;

/// <summary>
/// 定时任务抽象基类，封装 while 循环 + 异常处理 + Interval 等待。
/// 子类只需覆写 <see cref="Interval"/> 和 <see cref="InternalExecuteAsync"/>。
/// </summary>
public abstract class BaseBackgroundTaskService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly ILogger _loggerInstance;
    private static readonly CultureInfo DefaultCulture = new("zh-Hans");

    /// <summary>
    /// 子类可直接使用的 Logger（类型为子类自身）
    /// </summary>
    protected ILogger Logger => _loggerInstance;

    protected BaseBackgroundTaskService(ILogger logger)
    {
        _loggerInstance = logger;
    }

    /// <summary>
    /// 执行间隔
    /// </summary>
    protected abstract TimeSpan Interval { get; }

    /// <summary>
    /// 每次定时触发的具体业务逻辑
    /// </summary>
    protected abstract Task InternalExecuteAsync(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("{TaskName} 已启动", GetType().Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // 后台任务默认使用简体中文（不受 HTTP RequestLocalization 影响）
                CultureInfo.CurrentCulture = DefaultCulture;
                CultureInfo.CurrentUICulture = DefaultCulture;

                await InternalExecuteAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{TaskName} 执行异常", GetType().Name);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}
