using Ads.Automation.Domain.Pixel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ads.Automation.EntityFrameworkCore.Entity.Pixel;

public class AdsAccountPixelConfiguration : IEntityTypeConfiguration<AdsAccountPixel>
{
    public void Configure(EntityTypeBuilder<AdsAccountPixel> builder)
    {
        builder.ToTable("ads_account_pixel");

        // 主键（自增）
        builder.HasKey(p => p.Id);

        // 账户编号
        builder.Property(p => p.AccountNo).IsRequired().HasMaxLength(128);

        // 像素 ID
        builder.Property(p => p.PixelId).IsRequired();

        // 像素编号（冗余）
        builder.Property(p => p.PixelNo).HasMaxLength(128);

        // 索引：按账户查询像素
        builder.HasIndex(p => p.AccountNo);

        // 唯一索引：同一账户下同一像素不重复
        builder.HasIndex(p => new { p.AccountNo, p.PixelId }).IsUnique();
    }
}
