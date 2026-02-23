using IndigoAssits.API.Infrastructure.Legacy;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace IndigoAssits.API.Infrastructure.Swagger
{
    public sealed class SucursalHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            if (operation.Parameters.Any(p => p.Name == LegacyConnectionResolver.HeaderName))
            {
                return;
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = LegacyConnectionResolver.HeaderName,
                In = ParameterLocation.Header,
                Required = false,
                Description = "Sucursal para seleccionar la base legacy (ej. GDL)",
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
