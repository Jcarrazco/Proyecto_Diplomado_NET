using System.Net.Http.Json;
using IndigoAssits.Core.Dtos;
using IndigoAssistMVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IndigoAssistMVC.Services
{
    /// <summary>
    /// Servicio para consumir la API de Catálogos
    /// </summary>
    public class CatalogoApiService : ICatalogoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CatalogoApiService> _logger;
        private readonly string _apiBaseUrl;
        private readonly string _sucursal;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CatalogoApiService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<CatalogoApiService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5124";
            _sucursal = configuration["ApiSettings:Sucursal"] ?? "GDL";
            
            // Configurar la URL base del HttpClient
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Obtiene el token JWT de la sesión actual (si está disponible)
        /// </summary>
        private string? GetToken()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.Session != null)
                {
                    var token = httpContext.Session.GetString("ApiToken");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var expiresAtRaw = httpContext.Session.GetString("ApiTokenExpiresAt");
                        if (!string.IsNullOrWhiteSpace(expiresAtRaw) &&
                            DateTime.TryParse(expiresAtRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAt))
                        {
                            if (expiresAt <= DateTime.UtcNow)
                            {
                                _logger.LogWarning("Token JWT expirado en sesión. ExpiresAt: {ExpiresAt}", expiresAtRaw);
                                httpContext.Session.Remove("ApiToken");
                                httpContext.Session.Remove("ApiTokenExpiresAt");
                                return null;
                            }
                        }

                        _logger.LogDebug("Token encontrado en sesión");
                        return token;
                    }
                    else
                    {
                        _logger.LogWarning("No se encontró token en la sesión");
                    }
                }
                else
                {
                    _logger.LogWarning("HttpContext o Session no están disponibles");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo obtener el token de la sesión");
            }

            return null;
        }

        /// <summary>
        /// Crea un HttpRequestMessage con el token de autenticación si está disponible
        /// </summary>
        private HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri)
        {
            var request = new HttpRequestMessage(method, requestUri);
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("Token agregado al header Authorization");
            }
            else
            {
                _logger.LogWarning("No se agregó token al request {Method} {Uri}", method, requestUri);
            }

            if (!string.IsNullOrWhiteSpace(_sucursal))
            {
                request.Headers.TryAddWithoutValidation("X-Sucursal", _sucursal);
            }
            return request;
        }

        public async Task<List<TipoActivo>> GetTiposActivoAsync()
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, "/api/Catalogo/tipos-activo");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var tiposDto = await response.Content.ReadFromJsonAsync<List<TipoActivoDto>>();
                    return tiposDto?.Select(MapTipoActivo).ToList() ?? new List<TipoActivo>();
                }

                _logger.LogWarning("Error al obtener tipos de activo. Status: {StatusCode}", response.StatusCode);
                return new List<TipoActivo>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tipos de activo desde la API");
                return new List<TipoActivo>();
            }
        }

        public async Task<List<Status>> GetStatusActivoAsync()
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, "/api/Catalogo/status-activo");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var statusDto = await response.Content.ReadFromJsonAsync<List<StatusActivoDto>>();
                    return statusDto?.Select(MapStatus).ToList() ?? new List<Status>();
                }

                _logger.LogWarning("Error al obtener status de activos. Status: {StatusCode}", response.StatusCode);
                return new List<Status>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener status de activos desde la API");
                return new List<Status>();
            }
        }

        public async Task<List<Proveedor>> GetProveedoresAsync()
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, "/api/Catalogo/proveedores");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var proveedoresDto = await response.Content.ReadFromJsonAsync<List<ProveedorDto>>();
                    return proveedoresDto?.Select(MapProveedor).ToList() ?? new List<Proveedor>();
                }

                _logger.LogWarning("Error al obtener proveedores. Status: {StatusCode}", response.StatusCode);
                return new List<Proveedor>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proveedores desde la API");
                return new List<Proveedor>();
            }
        }

        public async Task<List<Componente>> GetComponentesAsync()
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, "/api/Catalogo/componentes");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var componentesDto = await response.Content.ReadFromJsonAsync<List<ComponenteDto>>();
                    return componentesDto?.Select(MapComponente).ToList() ?? new List<Componente>();
                }

                _logger.LogWarning("Error al obtener componentes. Status: {StatusCode}", response.StatusCode);
                return new List<Componente>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener componentes desde la API");
                return new List<Componente>();
            }
        }

        public async Task<List<mDepartamentos>> GetDepartamentosAsync()
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, "/api/Catalogo/departamentos");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var departamentosDto = await response.Content.ReadFromJsonAsync<List<DepartamentoDto>>();
                    return departamentosDto?.Select(MapDepartamento).ToList() ?? new List<mDepartamentos>();
                }

                _logger.LogWarning("Error al obtener departamentos. Status: {StatusCode}", response.StatusCode);
                return new List<mDepartamentos>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener departamentos desde la API");
                return new List<mDepartamentos>();
            }
        }

        #region Métodos de Mapeo

        private TipoActivo MapTipoActivo(TipoActivoDto dto)
        {
            return new TipoActivo
            {
                IdTipoActivo = dto.IdTipoActivo,
                TipoActivoNombre = dto.TipoActivoNombre
            };
        }

        private Status MapStatus(StatusActivoDto dto)
        {
            return new Status
            {
                StatusId = dto.StatusId,
                StatusNombre = dto.StatusNombre
            };
        }

        private Proveedor MapProveedor(ProveedorDto dto)
        {
            return new Proveedor
            {
                IdProveedor = dto.IdProveedor,
                ProveedorNombre = dto.ProveedorNombre
            };
        }

        private Componente MapComponente(ComponenteDto dto)
        {
            return new Componente
            {
                IdComponente = dto.IdComponente,
                ComponenteNombre = dto.ComponenteNombre,
                ValorBit = dto.ValorBit
            };
        }

        private mDepartamentos MapDepartamento(DepartamentoDto dto)
        {
            return new mDepartamentos
            {
                IdDepto = dto.IdDepto,
                Departamento = dto.Departamento,
                Tickets = dto.Tickets
            };
        }

        #endregion
    }
}

