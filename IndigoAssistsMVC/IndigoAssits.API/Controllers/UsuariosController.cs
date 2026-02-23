using IndigoAssits.API.Infrastructure.Legacy;
using IndigoAssits.API.Services;
using IndigoAssits.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;

namespace IndigoAssits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly ILegacyUserContextService _contextService;
        private readonly ILegacyDbConnectionFactory _legacyDbConnectionFactory;

        public UsuariosController(ILegacyUserContextService contextService, ILegacyDbConnectionFactory legacyDbConnectionFactory)
        {
            _contextService = contextService;
            _legacyDbConnectionFactory = legacyDbConnectionFactory;
        }

        [HttpGet("contexto")]
        public async Task<ActionResult<UserContextDto>> GetContexto()
        {
            var context = await _contextService.GetContextAsync(User);
            if (context == null)
            {
                return NotFound();
            }

            return Ok(context);
        }

        [HttpGet("tecnicos")]
        public async Task<ActionResult<IEnumerable<TecnicoDto>>> GetTecnicos([FromQuery] byte? departamentoId)
        {
            var result = new Dictionary<int, TecnicoDto>();

            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
SELECT e.IdPersona,
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
WHERE ISNULL(e.Activo, 1) = 1
  AND (@IdDepto IS NULL OR pu.IdDepto = @IdDepto)
ORDER BY d.Departamento, p.Nombre, p.Paterno, p.Materno";

            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };

            command.Parameters.AddWithValue("@IdDepto", (object?)departamentoId ?? DBNull.Value);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var idPersona = ReadInt32Safe(reader, "IdPersona");
                if (idPersona <= 0 || result.ContainsKey(idPersona))
                {
                    continue;
                }

                var nombre = ReadStringSafe(reader, "Nombre");
                var paterno = ReadStringSafe(reader, "Paterno");
                var materno = ReadStringSafe(reader, "Materno");
                var nombreCompleto = string.Join(" ", new[] { nombre, paterno, materno }
                    .Where(part => !string.IsNullOrWhiteSpace(part)));

                result[idPersona] = new TecnicoDto
                {
                    IdPersona = idPersona,
                    Login = ReadStringSafe(reader, "Login"),
                    NombreCompleto = nombreCompleto,
                    IdDepto = ReadByteSafe(reader, "IdDepto"),
                    Departamento = ReadStringSafe(reader, "Departamento")
                };
            }

            return Ok(result.Values.ToList());
        }

        public sealed class LegacyUserEnsureRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Login { get; set; } = string.Empty;
            public byte? IdDepto { get; set; }
        }

        [HttpPost("legacy/ensure")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EnsureLegacyUser([FromBody] LegacyUserEnsureRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Login))
            {
                return BadRequest("Email y Login son requeridos.");
            }

            if (request.Login.Length > 12)
            {
                return BadRequest("Login legacy no puede exceder 12 caracteres.");
            }

            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string findPersonaSql = @"
SELECT TOP 1 vp.IdPersona
FROM vPersonas vp
WHERE LOWER(vp.Email) = @Email";

            await using var findCommand = new SqlCommand(findPersonaSql, connection) { CommandType = CommandType.Text };
            findCommand.Parameters.AddWithValue("@Email", request.Email.ToLowerInvariant());
            var personaObj = await findCommand.ExecuteScalarAsync();

            if (personaObj == null || personaObj == DBNull.Value)
            {
                return NotFound("No se encontró la persona en legacy (vPersonas). Verifica el email.");
            }

            var idPersona = Convert.ToInt32(personaObj, CultureInfo.InvariantCulture);

            const string findEmpleadoSql = @"
SELECT COUNT(1)
FROM mEmpleados
WHERE IdPersona = @IdPersona";

            await using var countCommand = new SqlCommand(findEmpleadoSql, connection) { CommandType = CommandType.Text };
            countCommand.Parameters.AddWithValue("@IdPersona", idPersona);
            var exists = Convert.ToInt32(await countCommand.ExecuteScalarAsync(), CultureInfo.InvariantCulture) > 0;

            if (!exists)
            {
                await using var insertEmpleado = new SqlCommand(
                    "INSERT INTO mEmpleados (IdPersona, Login, Activo) VALUES (@IdPersona, @Login, 1)",
                    connection);
                insertEmpleado.Parameters.AddWithValue("@IdPersona", idPersona);
                insertEmpleado.Parameters.AddWithValue("@Login", request.Login);
                await insertEmpleado.ExecuteNonQueryAsync();
            }
            else
            {
                await using var updateEmpleado = new SqlCommand(
                    "UPDATE mEmpleados SET Login = @Login, Activo = 1 WHERE IdPersona = @IdPersona AND (Login IS NULL OR Login = '')",
                    connection);
                updateEmpleado.Parameters.AddWithValue("@IdPersona", idPersona);
                updateEmpleado.Parameters.AddWithValue("@Login", request.Login);
                await updateEmpleado.ExecuteNonQueryAsync();
            }

            var deptoId = request.IdDepto;
            if (deptoId.HasValue)
            {
                const string puestoSql = @"
SELECT TOP 1 IdPuesto
FROM mPuestos
WHERE IdDepto = @IdDepto
ORDER BY IdPuesto";

                await using var puestoCommand = new SqlCommand(puestoSql, connection) { CommandType = CommandType.Text };
                puestoCommand.Parameters.AddWithValue("@IdDepto", deptoId.Value);
                var puestoObj = await puestoCommand.ExecuteScalarAsync();

                if (puestoObj == null || puestoObj == DBNull.Value)
                {
                    return BadRequest("No existe un puesto para el departamento indicado.");
                }

                var idPuesto = Convert.ToByte(puestoObj, CultureInfo.InvariantCulture);

                const string existsDetalleSql = @"
SELECT COUNT(1)
FROM dEmpleados
WHERE IdPersona = @IdPersona";

                await using var existsDetalleCommand = new SqlCommand(existsDetalleSql, connection);
                existsDetalleCommand.Parameters.AddWithValue("@IdPersona", idPersona);
                var detalleExists = Convert.ToInt32(await existsDetalleCommand.ExecuteScalarAsync(), CultureInfo.InvariantCulture) > 0;

                if (!detalleExists)
                {
                    await using var insertDetalle = new SqlCommand(
                        "INSERT INTO dEmpleados (IdPersona, IdPuesto, Principal) VALUES (@IdPersona, @IdPuesto, 1)",
                        connection);
                    insertDetalle.Parameters.AddWithValue("@IdPersona", idPersona);
                    insertDetalle.Parameters.AddWithValue("@IdPuesto", idPuesto);
                    await insertDetalle.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { IdPersona = idPersona, Login = request.Login, IdDepto = deptoId });
        }

        private static int ReadInt32Safe(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return 0;
            }

            var value = reader.GetValue(ordinal);
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is short shortValue)
            {
                return shortValue;
            }

            if (value is long longValue)
            {
                return (int)longValue;
            }

            if (value is byte byteValue)
            {
                return byteValue;
            }

            if (value is string text && int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }

        private static byte? ReadByteSafe(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            var value = reader.GetValue(ordinal);
            if (value is byte byteValue)
            {
                return byteValue;
            }

            if (value is short shortValue)
            {
                return (byte)shortValue;
            }

            if (value is int intValue)
            {
                return (byte)intValue;
            }

            if (value is long longValue)
            {
                return (byte)longValue;
            }

            if (value is string text && byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
            {
                return parsed;
            }

            return Convert.ToByte(value, CultureInfo.InvariantCulture);
        }

        private static string ReadStringSafe(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return string.Empty;
            }

            var value = reader.GetValue(ordinal);
            return value?.ToString() ?? string.Empty;
        }
    }
}
