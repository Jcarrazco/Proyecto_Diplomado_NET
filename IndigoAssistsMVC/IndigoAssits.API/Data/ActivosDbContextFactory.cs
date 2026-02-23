using IndigoAssits.API.Infrastructure.Legacy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IndigoAssits.API.Data
{
    public sealed class ActivosDbContextFactory : IDesignTimeDbContextFactory<ActivosDbContext>
    {
        public ActivosDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetSection("LegacyConnectionStrings")["GDL"];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("No se encontró LegacyConnectionStrings:GDL para migraciones.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ActivosDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ActivosDbContext(optionsBuilder.Options);
        }
    }
}
