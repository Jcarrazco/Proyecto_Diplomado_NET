using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace IndigoAssits.API.Infrastructure.Legacy
{
    public interface ILegacyConnectionResolver
    {
        string CurrentSucursal { get; }
        string Resolve();
        string Resolve(string? sucursal);
    }

    public sealed class LegacyConnectionResolver : ILegacyConnectionResolver
    {
        public const string HeaderName = "X-Sucursal";
        private const string DefaultSucursal = "GDL";

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LegacyConnectionResolver(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string CurrentSucursal
        {
            get
            {
                var headerValue = _httpContextAccessor.HttpContext?.Request.Headers[HeaderName].ToString();
                return string.IsNullOrWhiteSpace(headerValue) ? DefaultSucursal : headerValue.Trim();
            }
        }

        public string Resolve()
        {
            return Resolve(CurrentSucursal);
        }

        public string Resolve(string? sucursal)
        {
            var key = string.IsNullOrWhiteSpace(sucursal) ? DefaultSucursal : sucursal.Trim();
            var normalizedKey = key.ToUpperInvariant();
            var legacySection = _configuration.GetSection("LegacyConnectionStrings");
            var connectionString = legacySection[normalizedKey];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = _configuration["ConnectionStrings:Indigo"];
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se pudo resolver la cadena de conexion legacy (LegacyConnectionStrings ni ConnectionStrings:Indigo).");
            }

            return connectionString;
        }
    }
}
