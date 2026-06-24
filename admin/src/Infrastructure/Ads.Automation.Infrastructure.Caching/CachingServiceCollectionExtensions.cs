
namespace Ads.Automation.Infrastructure.Caching
{
    public static class CachingServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCaching(this IServiceCollection services, IConfiguration configuration, string sectionName = "Redis")
        {
            services.Configure<RedisOptions>(configuration.GetSection(sectionName));

            services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<IDistributedLock, RedisDistributedLock>();
            services.AddSingleton<IRedisPubSubService, RedisPubSubService>();

            return services;
        }
    }
}
