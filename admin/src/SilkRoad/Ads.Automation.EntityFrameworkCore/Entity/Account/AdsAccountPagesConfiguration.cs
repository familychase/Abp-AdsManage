using Ads.Automation.Domain.Account;

namespace Ads.Automation.EntityFrameworkCore.Entity.Account;

public class AdsAccountPagesConfiguration : IEntityTypeConfiguration<AdsAccountPages>
{
    public void Configure(EntityTypeBuilder<AdsAccountPages> builder)
    {
        builder.ToTable("ads_account_pages");

        builder.Ignore(e => e.Id);

        // 复合主键
        builder.HasKey(e => new { e.AccountId, e.PageId });

        builder.Property(e => e.AccountNo).HasMaxLength(50);
    }
}
