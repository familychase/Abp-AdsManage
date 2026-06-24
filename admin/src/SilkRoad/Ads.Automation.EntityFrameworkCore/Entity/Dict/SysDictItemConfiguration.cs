namespace Ads.Automation.EntityFrameworkCore.Entity.Dict;

public class SysDictItemConfiguration : IEntityTypeConfiguration<SysDictItem>
{
    public void Configure(EntityTypeBuilder<SysDictItem> builder)
    {
        builder.ToTable("sys_dict_item");

        builder.Property(i => i.DictItemCode).IsRequired().HasMaxLength(64);
        builder.Property(i => i.DictItemName).IsRequired().HasMaxLength(128);
        builder.Property(i => i.DictItemNameEN).HasMaxLength(256);
        builder.Property(i => i.DictItemValue).IsRequired().HasMaxLength(256);
        builder.Property(i => i.Remarks).HasMaxLength(500);
        builder.Property(i => i.ItemType).HasConversion<byte>();

        builder.HasIndex(i => new { i.DictSortId, i.DictItemCode }).IsUnique();
    }
}
