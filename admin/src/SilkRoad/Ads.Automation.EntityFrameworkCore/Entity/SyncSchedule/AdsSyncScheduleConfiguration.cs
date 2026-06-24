using Ads.Automation.Domain.Shared.Enums;
using Ads.Automation.Domain.SyncSchedule;

namespace Ads.Automation.EntityFrameworkCore.Entity.SyncSchedule;

public class AdsSyncScheduleConfiguration : IEntityTypeConfiguration<AdsSyncSchedule>
{
    public void Configure(EntityTypeBuilder<AdsSyncSchedule> builder)
    {
        builder.ToTable("ads_sync_schedule");

        // 主键
        builder.HasKey(s => s.Id);

        // 动作类型
        builder.Property(s => s.ActionType).HasConversion<byte>().HasDefaultValue(ActionType.NONE);

        // 资源信息
        builder.Property(s => s.ResourceId).IsRequired().HasMaxLength(256).HasDefaultValue(string.Empty);
        builder.Property(s => s.ResourceType).HasConversion<byte>().HasDefaultValue(ResourceType.NONE);

        // 平台
        builder.Property(s => s.Platform).HasConversion<byte>().HasDefaultValue(PlatformType.NONE);

        // 任务
        builder.Property(s => s.JobName).IsRequired().HasMaxLength(256).HasDefaultValue(string.Empty);
        builder.Property(s => s.ExtendingData);

        // 层级与受众
        builder.Property(s => s.Level).HasDefaultValue(1);
        builder.Property(s => s.IsAudience).HasDefaultValue(false);
        builder.Property(s => s.LinkDate);

        // 发布时间
        builder.Property(s => s.NextPublishTime);

        // 索引
        builder.HasIndex(s => s.ResourceId).HasDatabaseName("idx_resource_id");
        builder.HasIndex(s => s.JobName).HasDatabaseName("idx_job_name");
        builder.HasIndex(s => s.NextPublishTime).HasDatabaseName("idx_next_publish_time");
    }
}
