using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IndigoAsists.Repositorio.Db
{
    public sealed class IndigoDbContextFactory : IDesignTimeDbContextFactory<IndigoDbContext>
    {
        public IndigoDbContext CreateDbContext(string[] args)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var basePath = currentDirectory;

            var solutionRoot = currentDirectory;
            while (solutionRoot != null
                && !File.Exists(Path.Combine(solutionRoot, "IndigoAssistMVC", "appsettings.json"))
                && !File.Exists(Path.Combine(solutionRoot, "IndigoAssits.API", "appsettings.json")))
            {
                var parent = Directory.GetParent(solutionRoot);
                if (parent == null) break;
                solutionRoot = parent.FullName;
            }

            var mvcSettings = solutionRoot == null
                ? null
                : Path.Combine(solutionRoot, "IndigoAssistMVC", "appsettings.json");
            var apiSettings = solutionRoot == null
                ? null
                : Path.Combine(solutionRoot, "IndigoAssits.API", "appsettings.json");

            if (mvcSettings != null && File.Exists(mvcSettings))
            {
                basePath = Path.Combine(solutionRoot, "IndigoAssistMVC");
            }
            else if (apiSettings != null && File.Exists(apiSettings))
            {
                basePath = Path.Combine(solutionRoot, "IndigoAssits.API");
            }

            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = currentDirectory;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json");
            }

            var optionsBuilder = new DbContextOptionsBuilder<IndigoDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new IndigoDbContext(optionsBuilder.Options);
        }
    }
}
