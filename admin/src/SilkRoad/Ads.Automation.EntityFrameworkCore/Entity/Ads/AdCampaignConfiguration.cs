using Ads.Automation.Domain.Ads;

namespace Ads.Automation.EntityFrameworkCore.Entity.Ads;

public class AdCampaignConfiguration : IEntityTypeConfiguration<AdCampaignEntity>
{
    public void Configure(EntityTypeBuilder<AdCampaignEntity> builder)
    {
        builder.ToTable("ad_campaign");

        // 主键
        builder.HasKey(c => c.Id);

        // 账户信息
        builder.Property(c => c.AccountId);
        builder.Property(c => c.AccountNo);

        // 系列信息
        builder.Property(c => c.CampaignNo);
        builder.Property(c => c.CampaignName);

        // 状态与预算
        builder.Property(c => c.MediaState);
        builder.Property(c => c.BudgetType);
        builder.Property(c => c.Budget).HasColumnType("DECIMAL(18,4)");

        // 推广目标
        builder.Property(c => c.Objective);

        // 时间
        builder.Property(c => c.MediaCreateTime);
        builder.Property(c => c.CreationTime);
        builder.Property(c => c.LastModificationTime);

    }
}
