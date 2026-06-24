using Ads.Automation.Domain.Publishing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.EntityFrameworkCore.Entity.Publishing
{
    public class AdsPublishingTemplateConfiguration : IEntityTypeConfiguration<AdsPublishTemplate>
    {
        public void Configure(EntityTypeBuilder<AdsPublishTemplate> builder)
        {
            builder.ToTable("ads_publish_templates");

            builder.HasKey(e => e.Id);

            builder.Property(u => u.Platform).HasColumnName("Platform").HasConversion<byte>();

            builder.Property(u => u.PublishingAdType).HasColumnName("PublishingAdType").HasConversion<byte>();

            builder.OwnsOne(e => e.Statistics, opt =>
            {
                opt.Property(e => e.PublishAdCount).HasColumnName("PublishAdCount");
                opt.Property(e => e.PublishCount).HasColumnName("PublishCount");
            });

            builder.OwnsOne(e => e.BatchPublishingOptions, opt =>
            {
                opt.Property(e => e.PublishAverage).HasColumnName("BatchPublishAverage");
                opt.Property(e => e.MaxPublishCount).HasColumnName("BatchMaxPublishCount");
                opt.Property(e => e.PublishingType).HasColumnName("BatchPublishingType").HasConversion<int>();
            });

        }
    }
}
