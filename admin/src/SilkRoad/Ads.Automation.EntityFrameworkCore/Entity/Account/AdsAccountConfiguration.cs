using Ads.Automation.Domain.Account;
using Ads.Automation.Domain.Shared.Enums;

namespace Ads.Automation.EntityFrameworkCore.Entity.Account;

public class AdsAccountConfiguration : IEntityTypeConfiguration<AdsAccount>
{
    public void Configure(EntityTypeBuilder<AdsAccount> builder)
    {
        builder.ToTable("ads_account");

        // 主键
        builder.HasKey(a => a.Id);

        // 基本信息
        builder.Property(a => a.AccountNo);
        builder.Property(a => a.AccountName);
        builder.Property(a => a.AccountState).HasColumnType("TINYINT");
        builder.Property(a => a.MediaState);
        builder.Property(a => a.Balance).HasColumnType("DECIMAL(18,2)");
        builder.Property(a => a.Timezone);
        builder.Property(a => a.UtcTimezoneOffset);

        // 时间
        builder.Property(a => a.CreationTime);
        builder.Property(a => a.LastModificationTime);

        // 平台与归属
        builder.Property(a => a.Platform).HasConversion<byte>();
        builder.Property(a => a.OwnerId);
        builder.Property(a => a.OwnerTeamId);

        // 账户属性
        builder.Property(a => a.IsManager);
        builder.Property(a => a.Currency);
        builder.Property(a => a.IsLimit);
        builder.Property(a => a.MediaDisableReason);
        builder.Property(a => a.MediaCreatedTime);
        builder.Property(a => a.AccountRunningTime).HasColumnType("DECIMAL(18,2)");

        // 索引
        builder.HasIndex(a => a.AccountNo).HasDatabaseName("IX_ads_account_account_no");
        builder.HasIndex(a => a.AccountName).HasDatabaseName("IX_ads_account_account_name");
    }
}
