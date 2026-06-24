
using Ads.Automation.EntityFrameworkCore.Entity.Users;
using Ads.Automation.Infrastructure.Repository;

namespace Ads.Automation.EntityFrameworkCore
{
    [DependsOn(
        typeof(AdsAutomationDomainModule),
        typeof(AbpEntityFrameworkCoreSqlServerModule),
        typeof(AbpIdentityEntityFrameworkCoreModule)
    )]
    public class AdsAutomationEntityFrameworkCoreModule: AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<AdsAutomationDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
                options.AddRepository<SysUser, EFCoreSysUserRepository>();
                options.AddRepository<SysLogInfo, EFCoreSysLogInfoRepository>();
                options.AddRepository<SysLogWarning, EFCoreSysLogWarningRepository>();
                options.AddRepository<SysLogError, EFCoreSysLogErrorRepository>();
            });

            // 通用泛型注册：所有 IBaseRepository<TEntity> 自动映射到 EFCoreBaseRepository<TEntity>
            context.Services.AddTransient(typeof(IBaseRepository<>), typeof(EFCoreBaseRepository<>));

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseSqlServer();
            });
        }
    }
}
