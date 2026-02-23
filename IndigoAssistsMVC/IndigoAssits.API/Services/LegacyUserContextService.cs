using System.Data;
using System.Linq;
using System.Security.Claims;
using IndigoAssits.API.Infrastructure.Legacy;
using IndigoAssits.Core.Dtos;
using Microsoft.Data.SqlClient;

namespace IndigoAssits.API.Services
{
    public interface ILegacyUserContextService
    {
        Task<UserContextDto?> GetContextAsync(ClaimsPrincipal user);
    }

    public sealed class LegacyUserContextService : ILegacyUserContextService
    {
        private readonly ILegacyDbConnectionFactory _connectionFactory;
        private readonly ILegacyConnectionResolver _resolver;

        public LegacyUserContextService(ILegacyDbConnectionFactory connectionFactory, ILegacyConnectionResolver resolver)
        {
            _connectionFactory = connectionFactory;
            _resolver = resolver;
        }

        public async Task<UserContextDto?> GetContextAsync(ClaimsPrincipal user)
        {
            var login = ResolveLogin(user);
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
SELECT TOP 1
    e.IdPersona,
    e.Login,
    p.Nombre,
    p.Paterno,
    p.Materno,
    pu.IdDepto,
    d.Departamento
FROM mEmpleados e
LEFT JOIN mPerEmp pe ON e.IdPersona = pe.IdPersona
LEFT JOIN mPersonas p ON pe.Persona = p.Persona
LEFT JOIN dEmpleados de ON e.IdPersona = de.IdPersona
LEFT JOIN mPuestos pu ON de.IdPuesto = pu.IdPuesto
LEFT JOIN mDepartamentos d ON pu.IdDepto = d.IdDepto
WHERE LOWER(e.Login) = @Login
   OR LOWER(p.Email) = @Login
ORDER BY de.Principal DESC, de.IdPuesto";

            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };
            command.Parameters.AddWithValue("@Login", login.ToLowerInvariant());

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new UserContextDto
            {
                IdPersona = reader.GetInt32(reader.GetOrdinal("IdPersona")),
                Login = reader.IsDBNull(reader.GetOrdinal("Login")) ? string.Empty : reader.GetString(reader.GetOrdinal("Login")),
                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? string.Empty : reader.GetString(reader.GetOrdinal("Nombre")),
                Paterno = reader.IsDBNull(reader.GetOrdinal("Paterno")) ? string.Empty : reader.GetString(reader.GetOrdinal("Paterno")),
                Materno = reader.IsDBNull(reader.GetOrdinal("Materno")) ? string.Empty : reader.GetString(reader.GetOrdinal("Materno")),
                IdDepto = reader.IsDBNull(reader.GetOrdinal("IdDepto")) ? null : reader.GetByte(reader.GetOrdinal("IdDepto")),
                Departamento = reader.IsDBNull(reader.GetOrdinal("Departamento")) ? string.Empty : reader.GetString(reader.GetOrdinal("Departamento")),
                Roles = roles,
                Sucursal = _resolver.CurrentSucursal
            };
        }

        private static string? ResolveLogin(ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name) ??
                   user.FindFirstValue("unique_name") ??
                   user.FindFirstValue(ClaimTypes.Upn) ??
                   user.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
