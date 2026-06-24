namespace Ads.Automation.EntityFrameworkCore.Log;

public class SysLogConfiguration : IEntityTypeConfiguration<SysLog>
{
    public void Configure(EntityTypeBuilder<SysLog> builder)
    {
        builder.ToTable("sys_log");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Level)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(l => l.Logger)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(l => l.Message)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(l => l.Exception)
            .HasMaxLength(8000);

        builder.Property(l => l.RequestPath)
            .HasMaxLength(1024);

        builder.HasIndex(l => l.Level);
        builder.HasIndex(l => l.CreationTime);
        builder.HasIndex(l => l.Logger);
    }
}
