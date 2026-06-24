using Ads.Automation.Domain.Ads;

namespace Ads.Automation.EntityFrameworkCore.Entity.Ads;

public class AdConfiguration : IEntityTypeConfiguration<AdEntity>
{
    public void Configure(EntityTypeBuilder<AdEntity> builder)
    {
        builder.ToTable("ad");

        // 主键
        builder.HasKey(a => a.Id);

        // 账户信息
        builder.Property(a => a.AccountId);
        builder.Property(a => a.AccountNo);

        // 系列关联
        builder.Property(a => a.CampaignId);
        builder.Property(a => a.CampaignNo);

        // 广告组关联
        builder.Property(a => a.AdSetId);
        builder.Property(a => a.AdSetNo);

        // 广告信息
        builder.Property(a => a.AdNo);
        builder.Property(a => a.AdName);

        // 状态
        builder.Property(a => a.MediaState);

        // 创意与主页
        builder.Property(a => a.CreativeNo);
        builder.Property(a => a.PageNo);

        // 时间
        builder.Property(a => a.MediaCreateTime);
        builder.Property(a => a.CreationTime);
        builder.Property(a => a.LastModificationTime);

    }
}
