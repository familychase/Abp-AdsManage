using Ads.Automation.Domain.Channel;
using Ads.Automation.Domain.Shared.Enums;

namespace Ads.Automation.EntityFrameworkCore.Entity.Channel;

public class AuthAppConfiguration : IEntityTypeConfiguration<AuthApp>
{
    public void Configure(EntityTypeBuilder<AuthApp> builder)
    {
        builder.ToTable("auth_app");

        // 主键
        builder.HasKey(a => a.Id);

        // 字段映射
        builder.Property(a => a.Platform).HasConversion<byte>();
        builder.Property(a => a.AppId).IsRequired().HasMaxLength(255);
        builder.Property(a => a.AppKey).IsRequired().HasMaxLength(255);
        builder.Property(a => a.AppSecreat).IsRequired().HasMaxLength(500);

        // 审计字段
        builder.Ignore(a => a.CreatorId);
        builder.Property(a => a.CreationTime);
    }
}
