using Volo.Abp.Data;
using Volo.Abp.Identity.EntityFrameworkCore;

namespace Ads.Automation.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class AdsAutomationDbContext : AbpDbContext<AdsAutomationDbContext>
    {
        public AdsAutomationDbContext(DbContextOptions<AdsAutomationDbContext> options) : base(options)
        {
        }

        // 所有领域实体通过 IAdsAutomationEntity 标记接口自动发现并注册，
        // 新增实体只需继承 AggregateRootEntity，无需在此声明 DbSet<T>。

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureIdentity();

            modelBuilder.ConfigureBusinessSystem();
        }
    }
}
