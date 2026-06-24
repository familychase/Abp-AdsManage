using Ads.Automation.Domain.Roles;
using Ads.Automation.Domain.SysRolePower;

namespace Ads.Automation.EntityFrameworkCore.Entity.RolePower
{
    public class SysRolePowerConfiguration : IEntityTypeConfiguration<SysRolePower>
    {
        public void Configure(EntityTypeBuilder<SysRolePower> builder)
        {
            builder.ToTable("sys_role_power");

            // Id 由数据库自增（IDENTITY），无需应用侧生成
        }
    }
}
