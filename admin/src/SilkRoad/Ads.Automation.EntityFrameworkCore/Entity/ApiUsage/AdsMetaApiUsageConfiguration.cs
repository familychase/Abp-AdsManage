namespace Ads.Automation.EntityFrameworkCore.Entity.ApiUsage;

public class AdsMetaApiUsageConfiguration : IEntityTypeConfiguration<AdsMetaApiUsage>
{
    public void Configure(EntityTypeBuilder<AdsMetaApiUsage> builder)
    {
        builder.ToTable("ads_meta_api_usage");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.AccountNo).IsRequired().HasMaxLength(50);
        builder.Property(u => u.TimeSlot).IsRequired();
        builder.Property(u => u.TotalCalls).IsRequired();
        builder.Property(u => u.TotalPoints).IsRequired();
        builder.Property(u => u.MethodStats).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(u => u.RateLimitHits).IsRequired();
        builder.Property(u => u.LastCallTime);
        builder.Property(u => u.LastRateLimitTime);
        builder.Property(u => u.CreationTime).IsRequired();
        builder.Property(u => u.LastModificationTime);
        builder.HasIndex(u => new { u.AccountNo, u.TimeSlot }).IsUnique();
    }
}
