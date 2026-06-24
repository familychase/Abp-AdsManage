using Ads.Automation.Domain.Channel;

namespace Ads.Automation.EntityFrameworkCore.Entity.Channel;

public class AdsChannelAdAccountConfiguration : IEntityTypeConfiguration<AdsChannelAdAccount>
{
    public void Configure(EntityTypeBuilder<AdsChannelAdAccount> builder)
    {
        builder.ToTable("ads_channel_adaccounts");
        builder.Ignore(e => e.Id);

        // 复合主键
        builder.HasKey(ca => new { ca.AccountId, ca.ChannelId });

        // 字段映射
        builder.Property(ca => ca.AccountId);
        builder.Property(ca => ca.ChannelId);
        builder.Property(ca => ca.AccountNo);

    }
}
