using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.EntityFrameworkCore.Entity.Ads
{
    public class AdsPublishAudienceConfiguration : IEntityTypeConfiguration<AdsPublishAudience>
    {
        public void Configure(EntityTypeBuilder<AdsPublishAudience> builder)
        {
            builder.ToTable("ads_publish_audience");

            builder.Property(u => u.Type).HasColumnName("Type").HasConversion<byte>();

            builder.Property(u => u.Platform).HasColumnName("Platform").HasConversion<byte>();
        }
    }
}
