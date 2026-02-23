using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IndigoAssistMVC.Data
{
    /// <summary>
    /// Fábrica de diseño para crear el DbContext en tiempo de diseño (migraciones)
    /// sin necesidad de levantar toda la aplicación y sus servicios de DI.
    /// </summary>
    public class IndigoDBContextFactory : IDesignTimeDbContextFactory<IndigoDBContext>
    {
        public IndigoDBContext CreateDbContext(string[] args)
        {
            // Construir la configuración desde appsettings.json
            // Intentar diferentes rutas posibles según desde dónde se ejecute la migración
            var currentDirectory = Directory.GetCurrentDirectory();
            var basePath = currentDirectory;
            
            // Buscar el directorio que contiene appsettings.json
            // Si estamos en la raíz de la solución, buscar el proyecto MVC
            var solutionRoot = currentDirectory;
            while (solutionRoot != null && !File.Exists(Path.Combine(solutionRoot, "IndigoAssistMVC", "appsettings.json")))
            {
                var parent = Directory.GetParent(solutionRoot);
                if (parent == null) break;
                solutionRoot = parent.FullName;
            }
            
            if (solutionRoot != null && File.Exists(Path.Combine(solutionRoot, "IndigoAssistMVC", "appsettings.json")))
            {
                basePath = Path.Combine(solutionRoot, "IndigoAssistMVC");
            }
            
            // Si no encontramos appsettings.json, usar el directorio actual
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = currentDirectory;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            // Obtener la cadena de conexión
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json");
            }

            // Crear las opciones del DbContext
            var optionsBuilder = new DbContextOptionsBuilder<IndigoDBContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Crear y retornar el contexto
            return new IndigoDBContext(optionsBuilder.Options);
        }
    }
}

