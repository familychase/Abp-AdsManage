using Ads.Automation.Api.Middleware;
using Ads.Automation.Application.Contracts.Entity.Duplicate;
using Ads.Automation.Application.Contracts.IntegrationJobs;
using System.Text.Json.Serialization;

namespace Ads.Automation.Api
{
    [DependsOn(
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        typeof(AdsAutomationApplicationModule),
        typeof(AdsAutomationEntityFrameworkCoreModule)
    )]
    public class AdsAutomationModule : AbpModule
    {
        /// <summary>
        /// 加载服务
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

            context.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            context.Services.AddEndpointsApiExplorer();
            context.Services.AddSwaggerGen(options =>
            {
                // 配置 access_token 全局Header
                options.AddSecurityDefinition("AccessToken", new OpenApiSecurityScheme
                {
                    Description = "请输入登录后返回的 access_token",
                    Name = "access_token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "AccessToken"
                });

                options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference("AccessToken", doc, null), new List<string>() }
                });
            });

            // 注册当前登录用户上下文（Scoped，每个请求一个实例）
            context.Services.AddScoped<UserInfoContext>();

            // 注册本地化上下文（Scoped，每个请求一个实例，由 LocalizationMiddleware 填充）
            context.Services.AddScoped<LocalizationContext>();

            // 添加后台服务（定时任务）
            AddBackgroundService(context);

            // 注册 RabbitMQ 生产者（消费者已拆到 SyncJobService 独立 Worker）
            AddJobQueueService(context);
        }

        /// <summary>
        /// 加载中间件
        /// </summary>
        /// <param name="context"></param>
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            // 强制验证 Redis 连接，连接异常直接阻止程序启动，定时任务不会执行
            var redisProvider = context.ServiceProvider.GetRequiredService<IRedisConnectionProvider>();
            if (!redisProvider.IsConnected)
            {
                throw new InvalidOperationException("Redis 连接未建立，程序无法启动");
            }

            // 验证 RabbitMQ 连接（非阻塞：连接失败仅警告，不阻止启动）
            var logger = context.ServiceProvider.GetRequiredService<ILogger<AdsAutomationModule>>();
            try
            {
                var mqFactory = context.ServiceProvider.GetRequiredService<IRabbitMqConnectionFactory>();
                var connection = mqFactory.GetConnection();
                if (connection.IsOpen)
                {
                    logger.LogInformation("RabbitMQ 连接正常: {Host}:{Port}",
                        connection.Endpoint.HostName, connection.Endpoint.Port);
                }
                else
                {
                    logger.LogWarning("RabbitMQ 连接已建立但未打开，消息队列功能可能不可用");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "RabbitMQ 连接失败，消息队列功能不可用（不影响 API 正常服务）");
            }

            // 初始化 IdGenerator
            IdGenerator.SetWorkerId(1);

            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // 注册自定义中间件（必须在 UseRouting 之后，才能通过 context.GetEndpoint() 读取 [AllowAnonymous]）
            AddMiddleware(app);

            app.UseConfiguredEndpoints();
        }

        /// <summary>
        /// 注册自定义中间件
        /// </summary>
        /// <param name="app"></param>
        private void AddMiddleware(IApplicationBuilder app)
        {
            // 本地化中间件（读取 accept-language 设置 CurrentCulture）
            app.UseMiddleware<LocalizationMiddleware>();

            // Token 认证中间件（置于路由之后，才能通过 context.GetEndpoint() 读取 [AllowAnonymous]）
            app.UseMiddleware<TokenAuthMiddleware>();

            // 响应包装中间件（捕获所有异常并包装响应）
            app.UseMiddleware<ResponseWrapperMiddleware>();

        }

        /// <summary>
        /// 添加后台服务
        /// </summary>
        /// <param name="context"></param>
        private void AddBackgroundService(ServiceConfigurationContext context)
        {       
            // context.Services.AddHostedService<DuplicateBackgroundService>();
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            if (!hostingEnvironment.IsDevelopment())
            {
                // 复制定时调度：每 1 分钟扫描 PENDING → 推入 MQ（执行由 SyncJobService 消费）
                context.Services.AddHostedService<DuplicateBackgroundService>();

                // Meta Token 刷新任务：每 1 小时检查并刷新即将过期的 Token
                context.Services.AddHostedService<TokenRefreshBackgroundService>();
                
                // 同步计划调度：每 1 分钟扫描 ads_sync_schedule → 推入 MQ（执行由 SyncJobService 消费）
                context.Services.AddHostedService<SyncScheduleBackgroundService>();
            }
        }

        /// <summary>
        /// 注册 RabbitMQ 生产者（消费者已拆到 SyncJobService 独立 Worker）
        /// </summary>
        /// <param name="context"></param>
        private void AddJobQueueService(ServiceConfigurationContext context)
        {
            context.Services.AddJobQueue();
        }
    }
}
