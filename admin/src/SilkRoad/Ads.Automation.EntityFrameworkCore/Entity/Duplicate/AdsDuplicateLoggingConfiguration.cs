using Ads.Automation.Domain.Duplicate;

namespace Ads.Automation.EntityFrameworkCore.Entity.Duplicate;

public class AdsDuplicateLoggingConfiguration : IEntityTypeConfiguration<AdsDuplicateLogging>
{
    public void Configure(EntityTypeBuilder<AdsDuplicateLogging> builder)
    {
        builder.ToTable("ads_duplicate_logging");

        // 主键
        builder.HasKey(d => d.Id);

        // 复制来源
        builder.Property(d => d.DuplicateSource)
            .HasConversion<byte>();

        // 来源ID
        builder.Property(d => d.ResourceId);

        // 是否内部复制
        builder.Property(d => d.IsInternal);

        // 广告对象层级
        builder.Property(d => d.AdObjectLevel)
            .HasConversion<byte>();

        // 广告对象编号（媒体编号）
        builder.Property(d => d.AdObjectNo);

        // 账户编号
        builder.Property(d => d.AccountNo);

        // 目标账户号
        builder.Property(d => d.DuplicateAccountNo);

        // 公共主页编号
        builder.Property(d => d.PageNo);

        // 复制状态
        builder.Property(d => d.State)
            .HasConversion<byte>();

        // 复制结果内容
        builder.Property(d => d.DuplicateContent);

        // 计划执行时间
        builder.Property(d => d.ScheduleTime);

        // 结束时间
        builder.Property(d => d.EndTime);

        // 创建时间
        builder.Property(d => d.CreationTime);

        // 创建人
        builder.Property(d => d.CreatorId);

        // 扩展数据
        builder.Property(d => d.ExtendedData);
        
        // 扩展数据
        builder.Property(d => d.CopyNumber);
        
        // 错误信息
        builder.Property(d => d.ErrorMessage);
    }
}
