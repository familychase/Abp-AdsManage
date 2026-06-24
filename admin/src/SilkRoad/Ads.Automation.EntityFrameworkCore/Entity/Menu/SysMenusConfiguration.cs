using Ads.Automation.Domain.Menu;

namespace Ads.Automation.EntityFrameworkCore.Entity.Menu
{
    public class SysMenusConfiguration : IEntityTypeConfiguration<SysMenus>
    {
        public void Configure(EntityTypeBuilder<SysMenus> builder)
        {
            builder.ToTable("sys_menus");

            builder.HasKey(x => x.Id);

            builder.Property(u => u.MenuType).HasColumnName("MenuType").HasConversion<byte>();
        }
    }
}
