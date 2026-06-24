namespace Ads.Automation.EntityFrameworkCore.Entity.Dict;

public class SysDictSortConfiguration : IEntityTypeConfiguration<SysDictSort>
{
    public void Configure(EntityTypeBuilder<SysDictSort> builder)
    {
        builder.ToTable("sys_dict_sort");

        builder.Property(s => s.DictSortCode).IsRequired().HasMaxLength(64);
        builder.Property(s => s.DictSortName).IsRequired().HasMaxLength(128);
        builder.Property(s => s.Remarks).HasMaxLength(500);
        builder.Property(s => s.DictSortType).HasConversion<byte>();
        builder.Property(s => s.Platform).HasConversion<byte>();

        builder.HasIndex(s => s.DictSortCode).IsUnique();
    }
}
