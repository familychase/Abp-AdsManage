using Ads.Automation.Domain.Ads;

namespace Ads.Automation.EntityFrameworkCore.Entity.Ads;

public class AdSetConfiguration : IEntityTypeConfiguration<AdSetEntity>
{
    public void Configure(EntityTypeBuilder<AdSetEntity> builder)
    {
        builder.ToTable("ad_set");

        // 主键
        builder.HasKey(s => s.Id);

        // 账户信息
        builder.Property(s => s.AccountId);
        builder.Property(s => s.AccountNo);

        // 系列关联
        builder.Property(s => s.CampaignId);
        builder.Property(s => s.CampaignNo);

        // 广告组信息
        builder.Property(s => s.AdSetNo);
        builder.Property(s => s.AdSetName);

        // 状态与预算
        builder.Property(s => s.MediaState);
        builder.Property(s => s.Budget).HasColumnType("DECIMAL(18,4)");
        builder.Property(s => s.BudgetType);

        // 时间
        builder.Property(s => s.MediaCreateTime);
        builder.Property(s => s.CreationTime);
        builder.Property(s => s.LastModificationTime);

    }
}
