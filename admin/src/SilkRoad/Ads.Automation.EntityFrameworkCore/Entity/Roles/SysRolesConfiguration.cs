using Ads.Automation.Domain.Roles;

namespace Ads.Automation.EntityFrameworkCore.Entity.Roles
{
    public class SysRolesConfiguration : IEntityTypeConfiguration<SysRoles>
    {
        public void Configure(EntityTypeBuilder<SysRoles> builder)
        {
            builder.ToTable("sys_roles");
        }
    }
}
