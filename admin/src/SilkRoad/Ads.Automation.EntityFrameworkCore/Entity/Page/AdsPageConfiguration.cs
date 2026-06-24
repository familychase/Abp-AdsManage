using Ads.Automation.Domain.Page;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ads.Automation.EntityFrameworkCore.Entity.Page;

public class AdsPageConfiguration : IEntityTypeConfiguration<AdsPage>
{
    public void Configure(EntityTypeBuilder<AdsPage> builder)
    {
        builder.ToTable("ads_page");

        // 主键
        builder.HasKey(p => p.Id);

        // Meta 主页编号
        builder.Property(p => p.PageNo).IsRequired();

        // 主页名称
        builder.Property(p => p.PageName);

        // 主页分类
        builder.Property(p => p.Category);

        // 关联账户号
        builder.Property(p => p.AccountNo);

        // 媒体平台
        builder.Property(p => p.Platform).HasConversion<byte>();

        // 最后同步时间
        builder.Property(p => p.LastSyncTime);

        // 创建时间
        builder.Property(p => p.CreationTime);

        // 索引
        builder.HasIndex(p => p.PageNo).IsUnique();
        builder.HasIndex(p => p.AccountNo);
    }
}
