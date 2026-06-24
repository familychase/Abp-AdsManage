using Ads.Automation.Infrastructure.Caching;
using Ads.Automation.Infrastructure.RabbitMq.Extensions;
using Ads.Automation.Infrastructure.Yitter;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using System.Globalization;

namespace Ads.Automation.SyncJobService;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 后台任务默认使用简体中文
        var culture = new CultureInfo("zh-Hans");
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        // 在 ABP 模块初始化之前设置 IdGenerator（workerId=2，与 API 的 workerId=1 区分）
        IdGenerator.SetWorkerId(2);

        var builder = Host.CreateDefaultBuilder(args)
            // 使用 bin/publish 目录作为内容根（Content 文件已由 Domain.Shared 传播至此）
            // CreateDefaultBuilder 会自动加载 appsettings.json + appsettings.{Environment}.json
            .UseContentRoot(AppContext.BaseDirectory)
            // 显式集成 Autofac（与 API 项目 builder.Host.UseAutofac() 一致）
            .UseAutofac()
            .ConfigureServices((ctx, services) =>
            {
                services.AddApplication<AdsAutomationSyncJobModule>();
                // 添加 Redis 缓存服务（与 API 项目一致）
                services.AddRedisCaching(ctx.Configuration);
                services.AddRabbitMq(ctx.Configuration);
            });

        var host = builder.Build();
        await host.RunAsync();
    }
}
