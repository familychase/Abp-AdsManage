using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ads.Automation.EntityFrameworkCore
{
    public class AdsAutomationDbContextFactory : IDesignTimeDbContextFactory<AdsAutomationDbContext>
    {
        public AdsAutomationDbContext CreateDbContext(string[] args)
        {
            AdsAutomationEfCoreEntityExtensionMappings.Configure();
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<AdsAutomationDbContext>();
            builder.UseSqlServer(configuration.GetConnectionString("Default")); //("Server=localhost\\SQLEXPRESS;Database=EDY;Trusted_Connection=True;TrustServerCertificate=True;TrustServerCertificate=True");

            return new AdsAutomationDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
