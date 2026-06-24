using Volo.Abp;

namespace Ads.Automation.EntityFrameworkCore
{
    public static class AdsAutomationDbContextModelCreatingExtension
    {
        /// <summary>
        /// 配置业务系统：
        /// 通过 ApplyConfigurationsFromAssembly 自动发现所有 IEntityTypeConfiguration&lt;T&gt; 配置类，
        /// EF Core 内部会为每个配置类调用 modelBuilder.Entity&lt;T&gt;()，无需手动声明 DbSet&lt;T&gt;。
        /// 新增实体只需新建 XxxConfiguration : IEntityTypeConfiguration&lt;Xxx&gt; 即可。
        /// </summary>
        public static void ConfigureBusinessSystem(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.ApplyConfigurationsFromAssembly(typeof(AdsAutomationDbContextModelCreatingExtension).Assembly);
        }
    }
}
