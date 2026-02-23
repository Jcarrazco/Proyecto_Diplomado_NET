using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAsists.Repositorio.Db;
using IndigoAsists.Repositorio.Repositories;

namespace IndigoAsists.Repositorio
{
    /// <summary>
    /// Configuración de servicios para el proyecto de repositorio
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Registra todos los servicios del repositorio
        /// </summary>
        /// <param name="services">Colección de servicios</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <returns>Colección de servicios configurada</returns>
        public static IServiceCollection AddRepositorioServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar Entity Framework
            services.AddDbContext<IndigoDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(30);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            services.AddDbContext<IndigoLegacyDbContext>(options =>
            {
                var legacyConnection = configuration.GetSection("LegacyConnectionStrings")["GDL"]
                    ?? configuration.GetConnectionString("LegacyConnection")
                    ?? configuration.GetConnectionString("DefaultConnection");

                options.UseSqlServer(legacyConnection, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(30);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });

            // Registrar repositorios
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            // UsuarioRepository requiere UserManager, se registra con factory
            services.AddScoped<IUsuarioRepository>(sp =>
            {
                var context = sp.GetRequiredService<IndigoDbContext>();
                var userManager = sp.GetService<UserManager<Usuario>>();
                return new UsuarioRepository(context, userManager);
            });
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
            services.AddScoped<IActivoRepository, ActivoRepository>();

            // Registrar UnitOfWork con UserManager
            services.AddScoped<IUnitOfWork>(sp =>
            {
                var legacyContext = sp.GetRequiredService<IndigoLegacyDbContext>();
                var identityContext = sp.GetRequiredService<IndigoDbContext>();
                var userManager = sp.GetService<UserManager<Usuario>>();
                return new UnitOfWork(legacyContext, identityContext, userManager);
            });

            return services;
        }
    }
}
