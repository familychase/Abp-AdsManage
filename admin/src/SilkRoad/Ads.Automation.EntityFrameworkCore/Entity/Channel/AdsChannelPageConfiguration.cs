using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.EntityFrameworkCore.Entity.Channel
{
    public class AdsChannelPageConfiguration : IEntityTypeConfiguration<AdsChannelPage>
    {
        public void Configure(EntityTypeBuilder<AdsChannelPage> builder)
        {
            builder.ToTable("ads_channel_pages");

            builder.Ignore(e => e.Id);

            // 复合主键
            builder.HasKey(ca => new { ca.PageId, ca.ChannelId });

            // 字段映射
            builder.Property(ca => ca.PageId);
            builder.Property(ca => ca.ChannelId);
            builder.Property(ca => ca.PageNo);
        }
    }
}
