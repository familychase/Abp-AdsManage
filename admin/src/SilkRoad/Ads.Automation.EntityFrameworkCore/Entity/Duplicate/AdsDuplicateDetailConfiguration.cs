using Ads.Automation.Domain.Duplicate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ads.Automation.EntityFrameworkCore.Entity.Duplicate;

public class AdsDuplicateDetailConfiguration : IEntityTypeConfiguration<AdsDuplicateDetail>
{
    public void Configure(EntityTypeBuilder<AdsDuplicateDetail> builder)
    {
        builder.ToTable("ads_duplicate_detail");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.LogId);
        builder.Property(d => d.Index);
        builder.Property(d => d.AdObjectLevel).HasConversion<byte>();
        builder.Property(d => d.AdObjectNo);
        builder.Property(d => d.State).HasConversion<byte>();
        builder.Property(d => d.ErrorMessage);
        builder.Property(d => d.Content);
        builder.Property(d => d.CreateTime);

        // 索引：按 LogId 查询某一复制任务的全部明细
        builder.HasIndex(d => d.LogId).HasDatabaseName("IX_ads_duplicate_detail_LogId");

        // 索引：按 AdObjectNo 查询，用于后续补充/删除
        builder.HasIndex(d => d.AdObjectNo).HasDatabaseName("IX_ads_duplicate_detail_AdObjectNo");
    }
}
