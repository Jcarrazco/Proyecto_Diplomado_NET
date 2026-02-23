using System.Globalization;
using System.Net.Http.Json;
using IndigoAssits.Core.Dtos;
using IndigoAssistMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IndigoAssistMVC.Services
{
    public class TicketApiService : ITicketApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TicketApiService> _logger;
        private readonly string _apiBaseUrl;
        private readonly string _sucursal;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<TicketApiService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5124";
            _sucursal = configuration["ApiSettings:Sucursal"] ?? "GDL";

            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<List<TicketVista>> GetTicketsAsync(TicketFiltroDto? filtros = null)
        {
            try
            {
                var query = BuildQueryString(filtros);
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Tickets{query}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var paginado = await response.Content.ReadFromJsonAsync<TicketPaginadoDto>();
                    var tickets = paginado?.Tickets?.Select(MapDtoToTicketVista).ToList() ?? new List<TicketVista>();
                    _logger.LogInformation("Tickets obtenidos: {Count}", tickets.Count);
                    return tickets;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener tickets. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return new List<TicketVista>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets desde la API");
                throw;
            }
        }

        public async Task<List<TicketVista>> GetTicketsAbiertosAsync(TicketFiltroDto? filtros = null)
        {
            try
            {
                var query = BuildQueryString(filtros);
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Tickets/abiertos{query}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var ticketsDto = await response.Content.ReadFromJsonAsync<List<TicketResponseDto>>();
                    return ticketsDto?.Select(MapDtoToTicketVista).ToList() ?? new List<TicketVista>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener tickets abiertos. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return new List<TicketVista>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets abiertos desde la API");
                throw;
            }
        }

        public async Task<TicketResponseDto?> GetTicketByIdAsync(int idTicket)
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Tickets/{idTicket}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TicketResponseDto>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener ticket {Id}. Status: {StatusCode}, Response: {Error}", idTicket, response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ticket {Id} desde la API", idTicket);
                throw;
            }
        }

        public async Task<UserContextDto?> GetUsuarioContextoAsync()
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, "/api/Usuarios/contexto");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserContextDto>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener contexto de usuario. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener contexto de usuario desde la API");
                throw;
            }
        }

        public async Task<TicketDashboardDto?> GetDashboardAsync(string scope)
        {
            try
            {
                var scopeValue = string.IsNullOrWhiteSpace(scope) ? "depto" : scope;
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Tickets/dashboard?scope={Uri.EscapeDataString(scopeValue)}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TicketDashboardDto>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener dashboard. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dashboard desde la API");
                throw;
            }
        }

        public async Task<List<TecnicoDto>> GetTecnicosAsync(byte? departamentoId = null)
        {
            try
            {
                var query = departamentoId.HasValue ? $"?departamentoId={departamentoId.Value}" : string.Empty;
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Usuarios/tecnicos{query}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var tecnicos = await response.Content.ReadFromJsonAsync<List<TecnicoDto>>();
                    return tecnicos ?? new List<TecnicoDto>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener tecnicos. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return new List<TecnicoDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tecnicos desde la API");
                throw;
            }
        }

        public async Task<bool> AsignarTecnicoAsync(int idTicket, int tecnicoId, DateTime? fechaCompromiso = null)
        {
            try
            {
                var payload = new TicketAsignacionMultipleDto
                {
                    IdTicket = idTicket,
                    Tecnicos = new List<int> { tecnicoId },
                    FeCompromiso = fechaCompromiso
                };

                using var request = CreateRequestMessage(HttpMethod.Patch, $"/api/Tickets/{idTicket}/asignar");
                request.Content = JsonContent.Create(payload);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al asignar tecnico. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar tecnico desde la API");
                throw;
            }
        }

        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage(method, requestUri);

            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning("Token inválido o expirado al hacer request {Method} {Uri}", method, requestUri);
            }

            if (!string.IsNullOrWhiteSpace(_sucursal))
            {
                request.Headers.TryAddWithoutValidation("X-Sucursal", _sucursal);
            }

            return request;
        }

        private string? GetToken()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.Session == null)
            {
                return null;
            }

            var token = httpContext.Session.GetString("ApiToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var expiresAtRaw = httpContext.Session.GetString("ApiTokenExpiresAt");
            if (!string.IsNullOrWhiteSpace(expiresAtRaw) &&
                DateTime.TryParse(expiresAtRaw, null, DateTimeStyles.RoundtripKind, out var expiresAt))
            {
                if (expiresAt <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Token JWT expirado en sesión. ExpiresAt: {ExpiresAt}", expiresAtRaw);
                    httpContext.Session.Remove("ApiToken");
                    httpContext.Session.Remove("ApiTokenExpiresAt");
                    return null;
                }
            }

            return token;
        }

        private static string BuildQueryString(TicketFiltroDto? filtros)
        {
            if (filtros == null)
            {
                return string.Empty;
            }

            if (filtros.Pagina <= 0)
            {
                filtros.Pagina = 1;
            }

            if (filtros.TamañoPagina <= 0 || filtros.TamañoPagina < 50)
            {
                filtros.TamañoPagina = 200;
            }

            var queryParams = new List<string>();

            if (filtros.UsuarioSolicitante > 0)
                queryParams.Add($"UsuarioSolicitante={filtros.UsuarioSolicitante}");

            if (filtros.IdTecnico > 0)
                queryParams.Add($"IdTecnico={filtros.IdTecnico}");

            if (!string.IsNullOrWhiteSpace(filtros.Status))
                queryParams.Add($"Status={Uri.EscapeDataString(filtros.Status)}");

            if (filtros.StatusId.HasValue)
                queryParams.Add($"StatusId={filtros.StatusId.Value}");

            if (filtros.IdCategoria > 0)
                queryParams.Add($"IdCategoria={filtros.IdCategoria}");

            if (filtros.IdSubCategoria > 0)
                queryParams.Add($"IdSubCategoria={filtros.IdSubCategoria}");

            if (filtros.Prioridad > 0)
                queryParams.Add($"Prioridad={filtros.Prioridad}");

            if (filtros.IdTipoTicket > 0)
                queryParams.Add($"IdTipoTicket={filtros.IdTipoTicket}");

            if (filtros.IdDepartamento > 0)
                queryParams.Add($"IdDepartamento={filtros.IdDepartamento}");

            if (filtros.FechaInicio != default)
                queryParams.Add($"FechaInicio={filtros.FechaInicio:yyyy-MM-dd}");

            if (filtros.FechaFin != default)
                queryParams.Add($"FechaFin={filtros.FechaFin:yyyy-MM-dd}");

            if (!string.IsNullOrWhiteSpace(filtros.BusquedaTexto))
                queryParams.Add($"BusquedaTexto={Uri.EscapeDataString(filtros.BusquedaTexto)}");

            if (!string.IsNullOrWhiteSpace(filtros.Titulo))
                queryParams.Add($"Titulo={Uri.EscapeDataString(filtros.Titulo)}");

            if (!string.IsNullOrWhiteSpace(filtros.Descripcion))
                queryParams.Add($"Descripcion={Uri.EscapeDataString(filtros.Descripcion)}");

            queryParams.Add($"Pagina={filtros.Pagina}");
            queryParams.Add($"TamañoPagina={filtros.TamañoPagina}");

            return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        }

        private static TicketVista MapDtoToTicketVista(TicketResponseDto dto)
        {
            return new TicketVista
            {
                IdTicket = dto.IdTicket,
                IdSolicitante = dto.UsuarioSolicitante,
                Solicitante = dto.SolicitanteNombre,
                IdTecnico = dto.IdTecnico == 0 ? null : dto.IdTecnico,
                Tecnico = dto.TecnicoNombre ?? string.Empty,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Status = dto.Status,
                StatusDes = dto.StatusDescripcion,
                IdTipoTicket = dto.IdTipoTicket,
                IdPrioridad = dto.Prioridad,
                FeAlta = dto.FeAlta,
                FeAsignacion = dto.FeAsignacion == default ? null : dto.FeAsignacion,
                FeCompromiso = dto.FeCompromiso == default ? null : dto.FeCompromiso,
                FeCierre = dto.FeCierre == default ? null : dto.FeCierre,
                IdSubCategoria = dto.IdSubCategoria,
                SubCategoria = dto.SubCategoriaNombre,
                IdCategoria = dto.IdCategoria,
                Categoria = dto.CategoriaNombre,
                IdDepto = dto.IdDepartamento,
                Departamento = dto.DepartamentoNombre
            };
        }
    }
}
