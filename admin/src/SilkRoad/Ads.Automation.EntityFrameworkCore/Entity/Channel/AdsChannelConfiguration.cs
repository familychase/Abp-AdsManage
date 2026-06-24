using Ads.Automation.Domain.Channel;
using Ads.Automation.Domain.Shared.Enums;

namespace Ads.Automation.EntityFrameworkCore.Entity.Channel;

public class AdsChannelConfiguration : IEntityTypeConfiguration<AdsChannel>
{
    public void Configure(EntityTypeBuilder<AdsChannel> builder)
    {
        builder.ToTable("ads_channel");

        // 主键
        builder.HasKey(c => c.Id);

        // 基本信息
        builder.Property(c => c.ChannelName).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Platform).HasConversion<byte>();
        builder.Property(c => c.ChannelState).HasConversion<byte>();
        builder.Property(c => c.AuditType).HasConversion<byte>().HasDefaultValue(AuditType.NO_SETTING);

        // 经理账户
        builder.Property(c => c.ManagerId).HasMaxLength(200);
        builder.Property(c => c.IsManager).HasDefaultValue(false);

        // 授权凭证
        builder.Property(c => c.AppKey).HasMaxLength(500);
        builder.Property(c => c.AppSecret).HasMaxLength(500);
        builder.Property(c => c.AccessToken).HasMaxLength(2000);
        builder.Property(c => c.RefreshToken).HasMaxLength(2000);

        // 其他信息
        builder.Property(c => c.MediaUserId).HasMaxLength(200);
        builder.Property(c => c.AuthMissingReason).HasMaxLength(2000);

        // 审计字段
        builder.Property(c => c.CreatorId);
        builder.Property(c => c.CreationTime);
        builder.Property(c => c.LastModifierId);
        builder.Property(c => c.LastModificationTime);

        // 软删除字段
        builder.Property(c => c.IsDeleted).HasDefaultValue(false);
        builder.Property(c => c.DeleterId);
        builder.Property(c => c.DeletionTime);

        // 索引
        builder.HasIndex(c => c.Platform).HasDatabaseName("IX_ads_channel_platform");
        builder.HasIndex(c => c.IsDeleted).HasDatabaseName("IX_ads_channel_deleted");
    }
}
