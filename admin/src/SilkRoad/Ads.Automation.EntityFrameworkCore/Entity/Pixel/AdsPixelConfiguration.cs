using Ads.Automation.Domain.Pixel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ads.Automation.EntityFrameworkCore.Entity.Pixel;

public class AdsPixelConfiguration : IEntityTypeConfiguration<AdsPixel>
{
    public void Configure(EntityTypeBuilder<AdsPixel> builder)
    {
        builder.ToTable("ads_pixel");

        // 主键
        builder.HasKey(p => p.Id);

        // Meta 像素编号（唯一）
        builder.Property(p => p.PixelNo).IsRequired().HasMaxLength(128);

        // 像素名称
        builder.Property(p => p.PixelName).HasMaxLength(256);

        // 像素追踪代码
        builder.Property(p => p.Code);

        // 最后同步时间
        builder.Property(p => p.LastSyncTime);

        // 创建时间
        builder.Property(p => p.CreationTime);

        // 索引
        builder.HasIndex(p => p.PixelNo).IsUnique();
    }
}
