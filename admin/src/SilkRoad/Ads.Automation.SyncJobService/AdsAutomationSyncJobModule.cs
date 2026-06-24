using Ads.Automation.Application;
using Ads.Automation.Application.Contracts.Entity.Duplicate;
using Ads.Automation.Application.Contracts.IntegrationJobs;
using Ads.Automation.Application.Statistic;
using Ads.Automation.Domain;
using Ads.Automation.Domain.Shared;
using Ads.Automation.Domain.Shared.Common;
using Ads.Automation.EntityFrameworkCore;
using Ads.Automation.Infrastructure.RabbitMq.Extensions;
using Ads.Automation.Infrastructure.Yitter;
using Ads.Automation.SyncJobService.SyncAdAccount;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Ads.Automation.SyncJobService;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AdsAutomationApplicationModule),
    typeof(AdsAutomationDomainModule),
    typeof(AbpAutomationDomainSharedModule),
    typeof(AdsAutomationEntityFrameworkCoreModule)
)]
public class AdsAutomationSyncJobModule : AbpModule
{
    /// <summary>
    /// 加载服务
    /// </summary>
    /// <param name="context"></param>
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // 注册当前登录用户上下文（与 API 项目一致）
        context.Services.AddScoped<UserInfoContext>();
        // 注册简单工厂服务
        context.Services.AddScoped<SyncReportingFactory>();
        // SyncAdAccountJobArgs → SyncAdAccountJobHandler（生产者+消费者）
        context.Services.AddRabbitMqConsumer<SyncAdAccountJobArgs, SyncAdAccountJobHandler>();
        // DuplicateTaskMessage → DuplicateJobHandler（仅消费者，生产者是 API 的 DuplicateBackgroundService）
        context.Services.AddRabbitMqConsumer<DuplicateTaskMessage, DuplicateJobHandler>();
        // 同步主页任务消费者（生产者在 API 项目中 SyncAdAccountJobHandler 同步账户后发送）
        context.Services.AddRabbitMqConsumer<SyncAdPageJobArgs, SyncAdPageJobHandler>();
        context.Services.AddRabbitMqConsumer<SyncAdPageAccountJobArgs, SyncAdPageAccountJobHandler>();
        // 广告结构同步（广告系列/广告组/广告）
        context.Services.AddRabbitMqConsumer<SyncAdCampaignJobArgs, SyncAdCampaignJobHandler>();
        // 广告报表同步（基础报表/消耗数据）
        context.Services.AddRabbitMqConsumer<SyncAdReportJobArgs, SyncAdReportJobHandler>();
        // 广告像素同步
        context.Services.AddRabbitMqConsumer<SyncAdPixelJobArgs, SyncAdPixelJobHandler>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // 初始化 IdGenerator（workerId=2，与 API 的 workerId=1 区分，避免 ID 冲突）
        IdGenerator.SetWorkerId(2);
    }
}
