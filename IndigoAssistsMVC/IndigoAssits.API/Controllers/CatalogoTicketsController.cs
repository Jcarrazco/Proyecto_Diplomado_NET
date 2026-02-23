using IndigoAssits.API.Infrastructure.Legacy;
using IndigoAssits.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace IndigoAssits.API.Controllers
{
    [Route("api/catalogo/tickets")]
    [ApiController]
    [Authorize]
    public class CatalogoTicketsController : ControllerBase
    {
        private readonly ILegacyDbConnectionFactory _legacyDbConnectionFactory;

        public CatalogoTicketsController(ILegacyDbConnectionFactory legacyDbConnectionFactory)
        {
            _legacyDbConnectionFactory = legacyDbConnectionFactory;
        }

        [HttpGet("status")]
        public async Task<ActionResult<IEnumerable<EstadoTicketDto>>> GetStatus()
        {
            const string sql = "SELECT Status, StatusDes FROM mStatusTicket ORDER BY Status";
            var result = new List<EstadoTicketDto>();

            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new EstadoTicketDto
                {
                    Status = ReadByteSafe(reader, "Status"),
                    StatusDes = ReadStringSafe(reader, "StatusDes")
                });
            }

            return Ok(result);
        }

        [HttpGet("prioridades")]
        public async Task<ActionResult<IEnumerable<PrioridadTicketDto>>> GetPrioridades()
        {
            const string sql = "SELECT IdPrioridad, Prioridad FROM mPrioridadTicket ORDER BY IdPrioridad";
            var result = new List<PrioridadTicketDto>();

            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new PrioridadTicketDto
                {
                    IdPrioridad = ReadByteSafe(reader, "IdPrioridad"),
                    Prioridad = ReadStringSafe(reader, "Prioridad")
                });
            }

            return Ok(result);
        }

        [HttpGet("tipos")]
        public async Task<ActionResult<IEnumerable<TipoTicketDto>>> GetTipos()
        {
            const string sql = "SELECT IdTipoTicket, TipoTicket FROM mTipoTicket ORDER BY IdTipoTicket";
            var result = new List<TipoTicketDto>();

            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new TipoTicketDto
                {
                    IdTipoTicket = ReadByteSafe(reader, "IdTipoTicket"),
                    TipoTicket = ReadStringSafe(reader, "TipoTicket")
                });
            }

            return Ok(result);
        }

        [HttpGet("categorias")]
        public async Task<ActionResult<IEnumerable<CategoriaTicketDto>>> GetCategorias()
        {
            const string sql = @"
SELECT c.IdCategoria, c.Categoria, c.IdDepto, d.Departamento
FROM mCategoriasTicket c
LEFT JOIN mDepartamentos d ON d.IdDepto = c.IdDepto
ORDER BY c.IdCategoria";

            var result = new List<CategoriaTicketDto>();

            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new CategoriaTicketDto
                {
                    IdCategoria = ReadByteSafe(reader, "IdCategoria"),
                    Categoria = ReadStringSafe(reader, "Categoria"),
                    IdDepto = ReadByteSafe(reader, "IdDepto"),
                    DepartamentoNombre = ReadStringSafe(reader, "Departamento")
                });
            }

            return Ok(result);
        }

        [HttpGet("subcategorias")]
        public async Task<ActionResult<IEnumerable<SubCategoriaTicketDto>>> GetSubCategorias()
        {
            const string sql = @"
SELECT s.IdSubCategoria, s.SubCategoria, s.IdCategoria, c.Categoria
FROM mSubCategoriasTicket s
LEFT JOIN mCategoriasTicket c ON c.IdCategoria = s.IdCategoria
ORDER BY s.IdSubCategoria";

            return Ok(await QuerySubCategoriasAsync(sql, null));
        }

        [HttpGet("subcategorias/{idCategoria:int}")]
        public async Task<ActionResult<IEnumerable<SubCategoriaTicketDto>>> GetSubCategoriasPorCategoria(int idCategoria)
        {
            if (idCategoria < 0 || idCategoria > 255)
            {
                return BadRequest("El ID de categoría debe estar entre 0 y 255");
            }

            const string sql = @"
SELECT s.IdSubCategoria, s.SubCategoria, s.IdCategoria, c.Categoria
FROM mSubCategoriasTicket s
LEFT JOIN mCategoriasTicket c ON c.IdCategoria = s.IdCategoria
WHERE s.IdCategoria = @IdCategoria
ORDER BY s.IdSubCategoria";

            var parameter = new SqlParameter("@IdCategoria", (byte)idCategoria);
            return Ok(await QuerySubCategoriasAsync(sql, parameter));
        }

        private async Task<List<SubCategoriaTicketDto>> QuerySubCategoriasAsync(string sql, SqlParameter? parameter)
        {
            var result = new List<SubCategoriaTicketDto>();
            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
            if (parameter != null)
            {
                command.Parameters.Add(parameter);
            }

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new SubCategoriaTicketDto
                {
                    IdSubCategoria = ReadByteSafe(reader, "IdSubCategoria"),
                    SubCategoria = ReadStringSafe(reader, "SubCategoria"),
                    IdCategoria = ReadByteSafe(reader, "IdCategoria"),
                    CategoriaNombre = ReadStringSafe(reader, "Categoria")
                });
            }

            return result;
        }

        private static byte ReadByteSafe(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return 0;
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
