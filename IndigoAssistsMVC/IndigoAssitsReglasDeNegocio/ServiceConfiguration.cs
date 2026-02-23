using IndigoAssitsReglasDeNegocio.Interfaces;
using IndigoAssitsReglasDeNegocio.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IndigoAssitsReglasDeNegocio
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection AddReglasDeNegocio(this IServiceCollection services)
        {
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IActivoService, ActivoService>();
            return services;
        }
    }
}


