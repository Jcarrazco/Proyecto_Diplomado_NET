using IndigoAssits.Core.Dtos;
using IndigoAssits.API.Infrastructure.Legacy;
using IndigoAssits.API.Services;
using IndigoAssitsReglasDeNegocio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace IndigoAssits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILegacyUserContextService _userContextService;
        private readonly ILegacyDbConnectionFactory _legacyDbConnectionFactory;

        public TicketsController(
            ITicketService ticketService,
            ILegacyUserContextService userContextService,
            ILegacyDbConnectionFactory legacyDbConnectionFactory)
        {
            _ticketService = ticketService;
            _userContextService = userContextService;
            _legacyDbConnectionFactory = legacyDbConnectionFactory;
        }

        [HttpGet]
        public async Task<ActionResult<TicketPaginadoDto>> Get([FromQuery] TicketFiltroDto filtros)
        {
            var result = await _ticketService.GetTicketsPaginadosAsync(filtros);
            return Ok(result);
        }

        [HttpGet("abiertos")]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetAbiertos([FromQuery] TicketFiltroDto filtros)
        {
            filtros.Status = "abierto";
            var result = await _ticketService.GetTicketsAsync(filtros);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TicketResponseDto>> GetById(int id)
        {
            var result = await _ticketService.GetTicketPorIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] TicketCreateDto dto)
        {
            var id = await _ticketService.CrearTicketAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TicketUpdateDto dto)
        {
            if (id != dto.IdTicket) return BadRequest("Id inconsistente");
            var ok = await _ticketService.ActualizarTicketAsync(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/asignar")]
        public async Task<IActionResult> Asignar(int id, [FromBody] TicketAsignacionMultipleDto dto)
        {
            if (id != dto.IdTicket) return BadRequest("Id inconsistente");
            var ok = await _ticketService.AsignarTicketAsync(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPost("{id:int}/anotaciones")]
        public async Task<IActionResult> AgregarAnotacion(int id, [FromBody] TicketAnotacionCreateDto dto)
        {
            if (id != dto.IdTicket) return BadRequest("Id inconsistente");
            var ok = await _ticketService.AgregarAnotacionAsync(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/cerrar")]
        public async Task<IActionResult> Cerrar(int id)
        {
            var ok = await _ticketService.CerrarTicketAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/reabrir")]
        public async Task<IActionResult> Reabrir(int id)
        {
            var ok = await _ticketService.ReabrirTicketAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/desasignar")]
        public async Task<IActionResult> Desasignar(int id)
        {
            var ok = await _ticketService.DesasignarTicketAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id:int}/estado/{estado:int}")]
        public async Task<IActionResult> CambiarEstado(int id, int estado)
        {
            if (estado < 0 || estado > 255) return BadRequest("El valor del estado debe estar entre 0 y 255");
            var ok = await _ticketService.CambiarEstadoTicketAsync(id, (byte)estado);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("estadisticas")]
        public async Task<ActionResult<TicketEstadisticasDto>> Estadisticas([FromQuery] byte? idDepartamento)
        {
            var result = await _ticketService.GetEstadisticasAsync(idDepartamento);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<TicketDashboardDto>> Dashboard([FromQuery] string scope = "depto")
        {
            var context = await _userContextService.GetContextAsync(User);
            if (context == null)
            {
                return NotFound();
            }

            var listas = new TicketDashboardListasDto();
            var normalizedScope = scope?.Trim().ToLowerInvariant() ?? "depto";

            if (normalizedScope == "depto" || normalizedScope == "admin")
            {
                if (context.IdDepto.HasValue)
                {
                    listas.AbiertosDepto = await QueryTicketsAsync("Status = 1 AND IdDepto = @IdDepto", new SqlParameter("@IdDepto", context.IdDepto.Value));
                    listas.EnProcesoDepto = await QueryTicketsAsync("Status = 2 AND IdDepto = @IdDepto", new SqlParameter("@IdDepto", context.IdDepto.Value));
                }

                listas.Asignados = await QueryTicketsAsync("Status = 2 AND IdTecnico = @IdPersona", new SqlParameter("@IdPersona", context.IdPersona));
            }

            if (normalizedScope == "usuario")
            {
                listas.AbiertosUsuario = await QueryTicketsAsync("Status = 1 AND IdSolicitante = @IdPersona", new SqlParameter("@IdPersona", context.IdPersona));
                listas.EnProcesoUsuario = await QueryTicketsAsync("Status = 2 AND IdSolicitante = @IdPersona", new SqlParameter("@IdPersona", context.IdPersona));
            }

            if (normalizedScope == "asignados")
            {
                listas.Asignados = await QueryTicketsAsync("Status = 2 AND IdTecnico = @IdPersona", new SqlParameter("@IdPersona", context.IdPersona));
            }

            var totales = new TicketDashboardTotalesDto
            {
                Abiertos = listas.AbiertosDepto.Count + listas.AbiertosUsuario.Count,
                EnProceso = listas.EnProcesoDepto.Count + listas.EnProcesoUsuario.Count,
                Asignados = listas.Asignados.Count,
                Cerrados = 0
            };

            var result = new TicketDashboardDto
            {
                Contexto = context,
                Totales = totales,
                Listas = listas
            };

            return Ok(result);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> Buscar([FromQuery] string q)
        {
            var result = await _ticketService.BuscarTicketsAsync(q ?? string.Empty);
            return Ok(result);
        }

        [HttpGet("recientes")]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetRecientes([FromQuery] int cantidad = 10)
        {
            if (cantidad <= 0 || cantidad > 100) return BadRequest("La cantidad debe estar entre 1 y 100");
            var result = await _ticketService.GetTicketsRecientesAsync(cantidad);
            return Ok(result);
        }

        private async Task<List<TicketResponseDto>> QueryTicketsAsync(string whereClause, params SqlParameter[] parameters)
        {
            var result = new List<TicketResponseDto>();
            await using var connection = _legacyDbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            var sql = $@"
SELECT TOP (50)
    IdTicket,
    IdSolicitante,
    Solicitante,
    IdTecnico,
    Tecnico,
    Titulo,
    Descripcion,
    Status,
    StatusDes,
    IdTipoTicket,
    IdPrioridad,
    FeAlta,
    FeAsignacion,
    FeCompromiso,
    FeCierre,
    IdSubCategoria,
    SubCategoria,
    IdCategoria,
    Categoria,
    IdDepto,
    Departamento
FROM vTickets
WHERE {whereClause}
ORDER BY FeAlta DESC";

            await using var command = new SqlCommand(sql, connection)
            {
                CommandType = CommandType.Text
            };

            if (parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new TicketResponseDto
                {
                    IdTicket = ReadInt32Safe(reader, "IdTicket"),
                    UsuarioSolicitante = ReadInt32Safe(reader, "IdSolicitante"),
                    SolicitanteNombre = ReadStringSafe(reader, "Solicitante"),
                    IdTecnico = ReadInt32Safe(reader, "IdTecnico"),
                    TecnicoNombre = ReadStringSafe(reader, "Tecnico"),
                    Titulo = ReadStringSafe(reader, "Titulo"),
                    Descripcion = ReadStringSafe(reader, "Descripcion"),
                    Status = ReadByteSafe(reader, "Status"),
                    StatusDescripcion = ReadStringSafe(reader, "StatusDes"),
                    IdTipoTicket = ReadByteSafe(reader, "IdTipoTicket"),
                    Prioridad = ReadByteSafe(reader, "IdPrioridad"),
                    FeAlta = ReadDateTimeSafe(reader, "FeAlta"),
                    FeAsignacion = ReadDateTimeSafe(reader, "FeAsignacion"),
                    FeCompromiso = ReadDateTimeSafe(reader, "FeCompromiso"),
                    FeCierre = ReadDateTimeSafe(reader, "FeCierre"),
                    IdSubCategoria = ReadByteSafe(reader, "IdSubCategoria"),
                    SubCategoriaNombre = ReadStringSafe(reader, "SubCategoria"),
                    IdCategoria = ReadByteSafe(reader, "IdCategoria"),
                    CategoriaNombre = ReadStringSafe(reader, "Categoria"),
                    IdDepartamento = ReadByteSafe(reader, "IdDepto"),
                    DepartamentoNombre = ReadStringSafe(reader, "Departamento")
                });
            }

            return result;
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

        private static DateTime ReadDateTimeSafe(SqlDataReader reader, string column)
        {
            var ordinal = reader.GetOrdinal(column);
            if (reader.IsDBNull(ordinal))
            {
                return default;
            }

            var value = reader.GetValue(ordinal);
            if (value is DateTime dateTimeValue)
            {
                return dateTimeValue;
            }

            if (value is string text && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return parsed;
            }

            return Convert.ToDateTime(value, CultureInfo.InvariantCulture);
        }
    }
}
