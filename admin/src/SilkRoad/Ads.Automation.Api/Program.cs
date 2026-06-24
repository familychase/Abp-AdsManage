using Ads.Automation.Api;
using Ads.Automation.Api.Logging;
using Ads.Automation.Infrastructure.Caching;
using Ads.Automation.Api.Middleware;

namespace Ads.Automation.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                // 使用 bin/publish 目录作为内容根（Content 文件已由 Domain.Shared 传播至此）
                ContentRootPath = AppContext.BaseDirectory,
                Args = args
            });

            // 加载 ABP 模块（包含本地化配置）
            builder.Services.AddApplication<AdsAutomationModule>();

            // 添加 Redis 缓存服务
            builder.Services.AddRedisCaching(builder.Configuration);

            // 添加 RabbitMQ 消息队列
            builder.Services.AddRabbitMq(builder.Configuration);

            // 添加跨域支持（localhost:4000 用于本地开发）
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DevCors", policy =>
                {
                    policy.WithOrigins("http://localhost:4000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // 注册 HttpContextAccessor（DatabaseLogger 所需）
            builder.Services.AddHttpContextAccessor();

            // 注册数据库日志记录器（将日志持久化到数据库）
            builder.Services.AddSingleton<DatabaseLoggerProcessor>();
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.Services.AddSingleton<ILoggerProvider>(sp =>
                new DatabaseLoggerProvider(
                    sp.GetRequiredService<DatabaseLoggerProcessor>(),
                    sp.GetRequiredService<IHttpContextAccessor>()));
            // 在 ClearProviders 之后重新添加 ILoggerFactory/ILogger<T>，确保可以通过 DI 解析 ILogger<T>
            builder.Services.AddLogging();

            // 配置请求本地化（默认简体中文，支持英文）
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "zh-Hans", "en" };
                options.SetDefaultCulture(supportedCultures[0]);
                options.AddSupportedCultures(supportedCultures);
                options.AddSupportedUICultures(supportedCultures);
            });

            // 集成 Autofac
            builder.Host.UseAutofac();

            var app = builder.Build();

            // 请求本地化中间件（必须放在最前面，为所有下游设置 CultureInfo）
            app.UseRequestLocalization();

            // 启用跨域（必须在路由之前）
            app.UseCors("DevCors");

            // 初始化 ABP 中间件（包含 UseRouting）
            app.InitializeApplication();

            app.MapControllers();

            app.Run();
        }
    }
}
