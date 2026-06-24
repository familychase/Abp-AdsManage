namespace Ads.Automation.EntityFrameworkCore.Entity.Users
{
    public class SysUserConfiguration : IEntityTypeConfiguration<SysUser>
    {
        public void Configure(EntityTypeBuilder<SysUser> builder)
        {
            builder.ToTable("sys_user");

            builder.Property(u => u.UserName).IsRequired().HasMaxLength(UserConsts.MaxUserNameLength);

            builder.Property(u => u.UserCode).IsRequired().HasMaxLength(UserConsts.MaxUserCodeLength);

            builder.Property(u => u.AliasName).HasMaxLength(UserConsts.MaxAliasNameLength);

            builder.Property(u => u.PhoneNumber).HasMaxLength(UserConsts.MaxPhoneNumberLength);

            builder.Property(u => u.Email).HasMaxLength(UserConsts.MaxEmailLength);

            builder.Property(u => u.Status).HasColumnName("Status").HasConversion<byte>();

            builder.HasIndex(u => u.UserCode).IsUnique();
            builder.HasIndex(u => u.UserName);
        }
    }
}
