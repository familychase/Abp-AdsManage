using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.EntityFrameworkCore.Entity.Reporting
{
    public class AdcciCampaignBaseRptConfiguration : IEntityTypeConfiguration<AdCampaignBaseRpt>
    {
        public void Configure(EntityTypeBuilder<AdCampaignBaseRpt> builder)
        {
            builder.ToTable("ad_campaign_base_rpt");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(m => m.Platform).HasColumnName("platform").HasConversion<byte>();

            builder.Property(e => e.Spend).HasColumnType("decimal(18,6)");
            builder.Property(e => e.Purchase).HasColumnType("decimal(18,6)");
            builder.Property(e => e.PurchaseValue).HasColumnType("decimal(18,6)");
            builder.Property(e => e.CPC).HasColumnType("decimal(18,6)");
            builder.Property(e => e.CPCO).HasColumnType("decimal(18,6)");

        }
    }
}
