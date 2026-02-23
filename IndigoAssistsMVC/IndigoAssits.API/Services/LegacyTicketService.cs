using System.Data;
using System.Globalization;
using System.Security.Claims;
using IndigoAssits.API.Infrastructure.Legacy;
using IndigoAssits.Core.Dtos;
using IndigoAssitsReglasDeNegocio.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IndigoAssits.API.Services
{
    public sealed class LegacyTicketService : ITicketService
    {
        private readonly ILegacyDbConnectionFactory _connectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LegacyTicketService> _logger;

        public LegacyTicketService(
            ILegacyDbConnectionFactory connectionFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<LegacyTicketService> logger)
        {
            _connectionFactory = connectionFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<TicketPaginadoDto> GetTicketsPaginadosAsync(TicketFiltroDto filtros)
        {
            var tickets = (await GetTicketsAsync(filtros)).ToList();
            var page = filtros.Pagina <= 0 ? 1 : filtros.Pagina;
            var pageSize = filtros.TamañoPagina <= 0 ? 10 : filtros.TamañoPagina;
            var total = tickets.Count;
            var totalPaginas = (int)Math.Ceiling((double)total / pageSize);
            var items = tickets.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new TicketPaginadoDto
            {
                Tickets = items,
                TotalRegistros = total,
                PaginaActual = page,
                TotalPaginas = totalPaginas,
                TamañoPagina = pageSize
            };
        }

        public async Task<IEnumerable<TicketResponseDto>> GetTicketsAsync(TicketFiltroDto filtros)
        {
            var status = ResolveStatus(filtros);
            var searchTerm = ResolveSearchTerm(filtros);

            var tickets = new List<TicketResponseDto>();
            var conditions = new List<string>();
            var parameters = new List<SqlParameter>();

            if (status.HasValue && status.Value > 0)
            {
                conditions.Add("Status = @Status");
                parameters.Add(new SqlParameter("@Status", status.Value));
            }

            if (filtros.IdDepartamento > 0)
            {
                conditions.Add("IdDepto = @IdDepto");
                parameters.Add(new SqlParameter("@IdDepto", filtros.IdDepartamento));
            }

            if (filtros.IdTecnico > 0)
            {
                conditions.Add("IdTecnico = @IdTecnico");
                parameters.Add(new SqlParameter("@IdTecnico", filtros.IdTecnico));
            }

            if (filtros.UsuarioSolicitante > 0)
            {
                conditions.Add("IdSolicitante = @IdSolicitante");
                parameters.Add(new SqlParameter("@IdSolicitante", filtros.UsuarioSolicitante));
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                conditions.Add("(Titulo LIKE @Filtro OR Descripcion LIKE @Filtro OR Solicitante LIKE @Filtro OR Tecnico LIKE @Filtro)");
                parameters.Add(new SqlParameter("@Filtro", $"%{searchTerm}%"));
            }

            var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : string.Empty;
            var sql = $@"
SELECT TOP (200)
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
{whereClause}
ORDER BY FeAlta DESC";

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();
            await using var command = new SqlCommand(sql, connection) { CommandType = CommandType.Text };
            if (parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tickets.Add(new TicketResponseDto
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

            if (!string.IsNullOrWhiteSpace(filtros.Titulo))
            {
                tickets = tickets
                    .Where(t => t.Titulo.Contains(filtros.Titulo, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return tickets;
        }

        public async Task<TicketResponseDto?> GetTicketPorIdAsync(int idTicket)
        {
            var usuario = ResolveLegacyUser();
            var result = await GetTicketDetalleAsync(idTicket, false, usuario);
            if (result != null)
            {
                return result;
            }

            return await GetTicketDetalleAsync(idTicket, true, usuario);
        }

        public async Task<int> CrearTicketAsync(TicketCreateDto dto)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            var usuarioLogin = !string.IsNullOrWhiteSpace(dto.UsuarioSolicitanteLogin)
                ? dto.UsuarioSolicitanteLogin.Trim()
                : await GetLoginByIdPersonaAsync(connection, dto.UsuarioSolicitante);
            if (string.IsNullOrWhiteSpace(usuarioLogin))
            {
                throw new InvalidOperationException("No se encontró el login del solicitante para el ticket.");
            }

            var useHumanized = (!string.IsNullOrWhiteSpace(dto.SubCategoriaNombre)
                                || !string.IsNullOrWhiteSpace(dto.TipoTicketNombre)
                                || !string.IsNullOrWhiteSpace(dto.PrioridadNombre)
                                || !string.IsNullOrWhiteSpace(dto.CategoriaNombre)
                                || dto.IdSubCategoria <= 0);

            if (useHumanized)
            {
                if (string.IsNullOrWhiteSpace(dto.SubCategoriaNombre))
                {
                    throw new InvalidOperationException("La subcategoría es requerida para el alta humanizada.");
                }

                await using var commandHuman = new SqlCommand("dbo.usp_Tickets_Insert_Human", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                commandHuman.Parameters.AddWithValue("@Usuario", usuarioLogin);
                commandHuman.Parameters.AddWithValue("@SubCategoria", dto.SubCategoriaNombre ?? string.Empty);
                commandHuman.Parameters.AddWithValue("@Categoria", (object?)dto.CategoriaNombre ?? DBNull.Value);
                commandHuman.Parameters.AddWithValue("@TipoTicket", (object?)dto.TipoTicketNombre ?? DBNull.Value);
                commandHuman.Parameters.AddWithValue("@Prioridad", (object?)dto.PrioridadNombre ?? DBNull.Value);
                commandHuman.Parameters.AddWithValue("@Titulo", dto.Titulo ?? string.Empty);
                commandHuman.Parameters.AddWithValue("@Descripcion", dto.Descripcion ?? string.Empty);

                var outputHuman = new SqlParameter("@IdTicket", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Output
                };
                commandHuman.Parameters.Add(outputHuman);

                await commandHuman.ExecuteNonQueryAsync();

                return Convert.ToInt32(outputHuman.Value, CultureInfo.InvariantCulture);
            }

            await using var command = new SqlCommand("dbo.usp_Tickets_Insert", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Usuario", usuarioLogin);
            command.Parameters.AddWithValue("@IdSubCategoria", dto.IdSubCategoria);
            command.Parameters.AddWithValue("@Titulo", dto.Titulo);
            command.Parameters.AddWithValue("@Descripcion", dto.Descripcion);
            var output = new SqlParameter("@IdTicket", SqlDbType.BigInt)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(output);

            await command.ExecuteNonQueryAsync();

            var newId = Convert.ToInt32(output.Value);

            if (dto.IdTipoTicket > 0 || dto.Prioridad > 0)
            {
                await using var updateCommand = new SqlCommand("dbo.usp_Tickets_Update", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                updateCommand.Parameters.AddWithValue("@IdTicket", newId);
                updateCommand.Parameters.AddWithValue("@Titulo", dto.Titulo ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@IdSubCategoria", dto.IdSubCategoria);
                updateCommand.Parameters.AddWithValue("@IdTipoTicket", dto.IdTipoTicket == 0 ? string.Empty : dto.IdTipoTicket.ToString());
                updateCommand.Parameters.AddWithValue("@IdPrioridad", dto.Prioridad == 0 ? string.Empty : dto.Prioridad.ToString());
                updateCommand.Parameters.AddWithValue("@Descripcion", dto.Descripcion ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@FeCompromiso", string.Empty);

                await updateCommand.ExecuteNonQueryAsync();
            }

            return newId;
        }

        public async Task<bool> ActualizarTicketAsync(TicketUpdateDto dto)
        {
            var usuario = ResolveLegacyUser();
            var detalle = await GetTicketDetalleAsync(dto.IdTicket, false, usuario);
            if (detalle == null)
            {
                return false;
            }

            var titulo = string.IsNullOrWhiteSpace(dto.Titulo) ? detalle.Titulo : dto.Titulo;
            var descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? detalle.Descripcion : dto.Descripcion;
            var idSubCategoria = dto.IdSubCategoria ?? detalle.IdSubCategoria;
            var idTipoTicket = dto.IdTipoTicket ?? detalle.IdTipoTicket;
            var prioridad = dto.Prioridad ?? detalle.Prioridad;
            var feCompromiso = dto.FeCompromiso.HasValue
                ? dto.FeCompromiso.Value.ToString("dd/MM/yyyy")
                : detalle.FeCompromiso == default
                    ? string.Empty
                    : detalle.FeCompromiso.ToString("dd/MM/yyyy");

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand("dbo.usp_Tickets_Update", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdTicket", dto.IdTicket);
            command.Parameters.AddWithValue("@Titulo", titulo ?? string.Empty);
            command.Parameters.AddWithValue("@IdSubCategoria", idSubCategoria);
            command.Parameters.AddWithValue("@IdTipoTicket", idTipoTicket == 0 ? string.Empty : idTipoTicket.ToString());
            command.Parameters.AddWithValue("@IdPrioridad", prioridad == 0 ? string.Empty : prioridad.ToString());
            command.Parameters.AddWithValue("@Descripcion", descripcion ?? string.Empty);
            command.Parameters.AddWithValue("@FeCompromiso", feCompromiso ?? string.Empty);

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public Task<bool> AsignarTicketAsync(TicketAsignacionDto dto)
        {
            return AsignarTicketAsync(new TicketAsignacionMultipleDto
            {
                IdTicket = dto.IdTicket,
                Tecnicos = new List<int> { dto.IdTecnico },
                FeCompromiso = dto.FeCompromiso
            });
        }

        public async Task<bool> AsignarTicketAsync(TicketAsignacionMultipleDto dto)
        {
            if (dto.Tecnicos == null || dto.Tecnicos.Count == 0)
            {
                return false;
            }

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            foreach (var tecnicoId in dto.Tecnicos)
            {
                await using var command = new SqlCommand("dbo.usp_Tickets_AsignarTecnico", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@IdTicket", dto.IdTicket);
                command.Parameters.AddWithValue("@Tecnico", tecnicoId);
                await command.ExecuteNonQueryAsync();
            }

            if (dto.FeCompromiso.HasValue)
            {
                await UpdateFechaCompromisoAsync(connection, dto.IdTicket, dto.FeCompromiso.Value);
            }

            return true;
        }

        public async Task<bool> AgregarAnotacionAsync(TicketAnotacionCreateDto dto)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand("dbo.usp_Tickets_Anotaciones_Insert", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdTicket", dto.IdTicket);
            command.Parameters.AddWithValue("@Obvs", dto.Observacion);
            command.Parameters.AddWithValue("@Duracion", dto.Duracion);
            command.Parameters.AddWithValue("@Usuario", dto.Usuario);

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public Task<bool> DesasignarTicketAsync(int idTicket)
        {
            return EjecutarSqlAsync(
                "DELETE FROM dTicketsTecnicos WHERE IdTicket = @IdTicket",
                new SqlParameter("@IdTicket", idTicket));
        }

        public Task<bool> CambiarEstadoTicketAsync(int idTicket, byte nuevoEstado)
        {
            return nuevoEstado switch
            {
                3 => CerrarTicketAsync(idTicket),
                2 => ReabrirTicketAsync(idTicket),
                _ => EjecutarSqlAsync(
                    "UPDATE mTickets SET Status = @Status WHERE IdTicket = @IdTicket",
                    new SqlParameter("@Status", nuevoEstado),
                    new SqlParameter("@IdTicket", idTicket))
            };
        }

        public async Task<bool> CerrarTicketAsync(int idTicket)
        {
            return await EjecutarCambioStatusAsync(idTicket, 2) > 0;
        }

        public async Task<bool> ReabrirTicketAsync(int idTicket)
        {
            return await EjecutarCambioStatusAsync(idTicket, 3) > 0;
        }

        public async Task<TicketEstadisticasDto> GetEstadisticasAsync(byte? idDepartamento = null)
        {
            var dto = new TicketEstadisticasDto();

            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            dto.TotalAbiertos = await ExecuteScalarAsync(connection,
                "SELECT COUNT(1) FROM vTickets WHERE Status = 1" + (idDepartamento.HasValue ? " AND IdDepto = @IdDepto" : string.Empty),
                idDepartamento.HasValue ? new SqlParameter("@IdDepto", idDepartamento.Value) : null);

            dto.TotalEnProceso = await ExecuteScalarAsync(connection,
                "SELECT COUNT(1) FROM vTickets WHERE Status = 2" + (idDepartamento.HasValue ? " AND IdDepto = @IdDepto" : string.Empty),
                idDepartamento.HasValue ? new SqlParameter("@IdDepto", idDepartamento.Value) : null);

            dto.TotalCerrados = await ExecuteScalarAsync(connection,
                "SELECT COUNT(1) FROM vTickets WHERE Status = 3" + (idDepartamento.HasValue ? " AND IdDepto = @IdDepto" : string.Empty),
                idDepartamento.HasValue ? new SqlParameter("@IdDepto", idDepartamento.Value) : null);

            dto.PorDepartamento = await ExecuteDictionaryAsync(connection,
                "SELECT Departamento, COUNT(1) FROM vTickets GROUP BY Departamento");

            dto.PorPrioridad = await ExecuteDictionaryAsync(connection,
                "SELECT ISNULL(CONVERT(varchar(10), IdPrioridad), '0') AS Prioridad, COUNT(1) FROM vTickets GROUP BY IdPrioridad");

            dto.PorEstado = await ExecuteDictionaryAsync(connection,
                "SELECT StatusDes, COUNT(1) FROM vTickets GROUP BY StatusDes");

            return dto;
        }

        public async Task<IEnumerable<TicketResponseDto>> GetTicketsRecientesAsync(int cantidad = 10)
        {
            var result = new List<TicketResponseDto>();
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(
                "SELECT TOP (@Cantidad) IdTicket, Titulo, FeAlta, Departamento, Status, StatusDes, Solicitante, Tecnico FROM vTickets ORDER BY FeAlta DESC",
                connection);
            command.Parameters.AddWithValue("@Cantidad", cantidad);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new TicketResponseDto
                {
                    IdTicket = ReadInt32Safe(reader, "IdTicket"),
                    Titulo = ReadStringSafe(reader, "Titulo"),
                    FeAlta = ReadDateTimeSafe(reader, "FeAlta"),
                    DepartamentoNombre = ReadStringSafe(reader, "Departamento"),
                    Status = ReadByteSafe(reader, "Status"),
                    StatusDescripcion = ReadStringSafe(reader, "StatusDes"),
                    SolicitanteNombre = ReadStringSafe(reader, "Solicitante"),
                    TecnicoNombre = ReadStringSafe(reader, "Tecnico")
                });
            }

            return result;
        }

        public async Task<IEnumerable<TicketResponseDto>> BuscarTicketsAsync(string terminoBusqueda)
        {
            var filtros = new TicketFiltroDto
            {
                BusquedaTexto = terminoBusqueda
            };
            return await GetTicketsAsync(filtros);
        }

        private string ResolveLegacyUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userName = user?.FindFirstValue(ClaimTypes.Name) ??
                           user?.FindFirstValue("unique_name") ??
                           user?.FindFirstValue(ClaimTypes.Upn) ??
                           user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                return userName;
            }

            return _configuration["LegacyTickets:FallbackUser"] ?? "admin";
        }

        private static byte? ResolveStatus(TicketFiltroDto filtros)
        {
            if (filtros.StatusId.HasValue)
            {
                return filtros.StatusId.Value;
            }

            if (string.IsNullOrWhiteSpace(filtros.Status))
            {
                return null;
            }

            if (byte.TryParse(filtros.Status, out var numeric))
            {
                return numeric;
            }

            var normalized = filtros.Status.Trim().ToLowerInvariant();
            return normalized switch
            {
                "nuevo" => (byte)1,
                "abierto" => (byte)1,
                "asignado" => (byte)2,
                "enproceso" => (byte)2,
                "en_proceso" => (byte)2,
                "cerrado" => (byte)3,
                "todos" => (byte)0,
                _ => null
            };
        }

        private static string? ResolveSearchTerm(TicketFiltroDto filtros)
        {
            if (!string.IsNullOrWhiteSpace(filtros.BusquedaTexto))
            {
                return filtros.BusquedaTexto;
            }

            if (!string.IsNullOrWhiteSpace(filtros.Descripcion))
            {
                return filtros.Descripcion;
            }

            if (!string.IsNullOrWhiteSpace(filtros.Titulo))
            {
                return filtros.Titulo;
            }

            return null;
        }

        private static async Task<string?> GetTecnicoLoginByIdAsync(SqlConnection connection, int tecnicoId)
        {
            await using var command = new SqlCommand(
                "SELECT Login FROM mEmpleados WHERE IdPersona = @IdPersona",
                connection);
            command.Parameters.AddWithValue("@IdPersona", tecnicoId);
            var result = await command.ExecuteScalarAsync();
            return result?.ToString();
        }

        private static async Task<string?> GetLoginByIdPersonaAsync(SqlConnection connection, int personaId)
        {
            await using var command = new SqlCommand(
                "SELECT Login FROM mEmpleados WHERE IdPersona = @IdPersona",
                connection);
            command.Parameters.AddWithValue("@IdPersona", personaId);
            var result = await command.ExecuteScalarAsync();
            return result?.ToString();
        }

        private async Task<TicketResponseDto?> GetTicketDetalleAsync(int idTicket, bool historicos, string usuario)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand("dbo.usp_Tickets_Detalle", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@Historicos", historicos ? 1 : 0);
            command.Parameters.AddWithValue("@Usuario", usuario);
            command.Parameters.AddWithValue("@IdTicket", idTicket);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new TicketResponseDto
            {
                IdTicket = ReadInt32Safe(reader, "IdTicket"),
                UsuarioSolicitante = ReadInt32Safe(reader, "Solicitante"),
                SolicitanteNombre = ReadStringSafe(reader, "NomSolicitante"),
                Titulo = ReadStringSafe(reader, "Titulo"),
                Descripcion = ReadStringSafe(reader, "Descripcion"),
                IdSubCategoria = ReadByteSafe(reader, "IdSubCategoria"),
                SubCategoriaNombre = ReadStringSafe(reader, "SubCategoria"),
                IdCategoria = ReadByteSafe(reader, "IdCategoria"),
                CategoriaNombre = ReadStringSafe(reader, "Categoria"),
                IdDepartamento = ReadByteSafe(reader, "IdDepto"),
                DepartamentoNombre = ReadStringSafe(reader, "Departamento"),
                Status = ReadByteSafe(reader, "Status"),
                StatusDescripcion = ReadStringSafe(reader, "StatusDes"),
                IdTipoTicket = ReadByteSafe(reader, "IdTipoTicket"),
                TipoTicketNombre = ReadStringSafe(reader, "TipoTicket"),
                Prioridad = ReadByteSafe(reader, "IdPrioridad"),
                PrioridadNombre = ReadStringSafe(reader, "DescPrioridad"),
                FeAlta = ReadDateTimeSafe(reader, "FeAlta"),
                FeAsignacion = ReadDateTimeSafe(reader, "FeAsignacion"),
                FeCompromiso = ReadDateTimeSafe(reader, "FeCompromiso"),
                FeCierre = ReadDateTimeSafe(reader, "FeCierre")
            };
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

        private static int ReadInt32Safe(SqlDataReader reader, int ordinal)
        {
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

        private static async Task UpdateFechaCompromisoAsync(SqlConnection connection, int idTicket, DateTime fechaCompromiso)
        {
            await using var command = new SqlCommand(
                "UPDATE mTickets SET FeCompromiso = @FeCompromiso WHERE IdTicket = @IdTicket",
                connection);
            command.Parameters.AddWithValue("@FeCompromiso", fechaCompromiso);
            command.Parameters.AddWithValue("@IdTicket", idTicket);
            await command.ExecuteNonQueryAsync();
        }

        private async Task<int> EjecutarCambioStatusAsync(int idTicket, int opc)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand("dbo.usp_Tickets_ChangeStatus", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@IdTicket", idTicket);
            command.Parameters.AddWithValue("@Opc", opc);

            var output = new SqlParameter("@ValEliminaTicket", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(output);

            await command.ExecuteNonQueryAsync();
            return output.Value == DBNull.Value ? 0 : Convert.ToInt32(output.Value);
        }

        private async Task<bool> EjecutarSqlAsync(string sql, params SqlParameter[] parameters)
        {
            await using var connection = _connectionFactory.CreateConnection();
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            var result = await command.ExecuteNonQueryAsync();
            return result > 0;
        }

        private static async Task<int> ExecuteScalarAsync(SqlConnection connection, string sql, SqlParameter? parameter)
        {
            await using var command = new SqlCommand(sql, connection);
            if (parameter != null)
            {
                command.Parameters.Add(parameter);
            }

            var result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private static async Task<Dictionary<string, int>> ExecuteDictionaryAsync(SqlConnection connection, string sql)
        {
            var result = new Dictionary<string, int>();
            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var key = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                var value = reader.IsDBNull(1) ? 0 : ReadInt32Safe(reader, 1);
                result[key] = value;
            }

            return result;
        }
    }
}
