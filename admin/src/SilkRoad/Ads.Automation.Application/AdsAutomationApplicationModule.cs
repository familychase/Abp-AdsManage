namespace Ads.Automation.Application
{
    [DependsOn(
        typeof(AdsAutomationDomainModule),
        typeof(AdsAutomationApplicationContractModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
    )]
    public class AdsAutomationApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<AdsAutomationApplicationModule>();
            });

            context.Services.AddTransient<MetaErrorParser>();

            // Meta Token 加密服务（Singleton，基于固定 Guid 派生 AES 密钥）
            context.Services.AddSingleton<MetaTokenEncryptionService>();

            // 重试配置：默认值来自 MetaApiRetryOptions 类属性，appsettings.json 可覆盖
            var retryOptions = new MetaApiRetryOptions();
            context.Services.GetConfiguration().GetSection("MetaApiRetry").Bind((object)retryOptions);
            context.Services.AddSingleton(retryOptions);

            // 限流配置与限流器（基于 Redis 令牌桶，读/写分桶）
            var rateLimitOptions = new MetaApiRateLimitOptions();
            context.Services.GetConfiguration().GetSection("MetaApiRateLimit").Bind((object)rateLimitOptions);
            context.Services.AddSingleton(rateLimitOptions);
            context.Services.AddSingleton<MetaApiRateLimiter>();

            // 全局熔断器（Singleton — 所有 Policy 实例共享状态）
            context.Services.AddSingleton<MetaApiCircuitBreaker>();

            context.Services.AddTransient<MetaApiRetryPolicy>();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            // 触发 MetaApiUsageTracker 单例初始化，确保 OnHttpRequest / OnApiExecuted 回调被及时注册
            _ = context.ServiceProvider.GetRequiredService<MetaApiUsageTracker>();
            base.OnApplicationInitialization(context);
        }
    }
}
