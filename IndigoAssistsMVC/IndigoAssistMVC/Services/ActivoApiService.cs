using System.Net.Http.Json;
using System.Text.Json;
using IndigoAssits.Core.Dtos;
using IndigoAssistMVC.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace IndigoAssistMVC.Services
{
    /// <summary>
    /// Servicio para consumir la API de Activos
    /// </summary>
    public class ActivoApiService : IActivoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ActivoApiService> _logger;
        private readonly string _apiBaseUrl;
        private readonly string _sucursal;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ActivoApiService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<ActivoApiService> logger,
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
                    // Intentar obtener el token de la sesión
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

        public async Task<List<ActivoViewModel>> GetActivosAsync(ActivoFiltroViewModel? filtro = null)
        {
            try
            {
                _logger.LogInformation("GetActivosAsync llamado. Filtro es null: {IsNull}", filtro == null);
                
                var filtroDto = MapFiltroToDto(filtro);
                var queryParams = BuildQueryString(filtroDto);

                _logger.LogInformation("Query params construidos: '{QueryParams}'", queryParams);
                
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Activos/todos{queryParams}");
                
                _logger.LogInformation("Solicitando activos desde: {BaseUrl}/api/Activos/todos{QueryParams}", _apiBaseUrl, queryParams);
                
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var activosDto = await response.Content.ReadFromJsonAsync<List<ActivoDto>>();
                    _logger.LogInformation("Respuesta de la API recibida. activosDto es null: {IsNull}, Count: {Count}", 
                        activosDto == null, activosDto?.Count ?? 0);
                    
                    var activos = activosDto?.Select(MapDtoToViewModel).ToList() ?? new List<ActivoViewModel>();
                    _logger.LogInformation("Se obtuvieron {Count} activos desde la API después del mapeo", activos.Count);
                    return activos;
                }

                // Leer el contenido del error para diagnóstico
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al obtener activos. Status: {StatusCode}, Response: {ErrorContent}", 
                    response.StatusCode, errorContent);
                
                // Si es 401, el token puede haber expirado o no estar presente
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("No autorizado. Verificar que el token JWT esté presente y válido en la sesión.");
                }
                
                return new List<ActivoViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener activos desde la API: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<ActivoViewModel?> GetActivoPorIdAsync(int id)
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Get, $"/api/Activos/{id}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var activoDto = await response.Content.ReadFromJsonAsync<ActivoDto>();
                    return activoDto != null ? MapDtoToViewModel(activoDto) : null;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                _logger.LogWarning("Error al obtener activo {Id}. Status: {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener activo {Id} desde la API", id);
                throw;
            }
        }

        public async Task<int> CrearActivoAsync(ActivoViewModel viewModel)
        {
            try
            {
                var createDto = MapViewModelToCreateDto(viewModel);
                using var request = CreateRequestMessage(HttpMethod.Post, "/api/Activos");
                request.Content = System.Net.Http.Json.JsonContent.Create(createDto);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var id = await response.Content.ReadFromJsonAsync<int>();
                    return id;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al crear activo. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorContent);
                throw new HttpRequestException($"Error al crear activo: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear activo en la API");
                throw;
            }
        }

        public async Task<bool> ActualizarActivoAsync(int id, ActivoViewModel viewModel)
        {
            try
            {
                var updateDto = MapViewModelToUpdateDto(viewModel);
                using var request = CreateRequestMessage(HttpMethod.Put, $"/api/Activos/{id}");
                request.Content = System.Net.Http.Json.JsonContent.Create(updateDto);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al actualizar activo {Id}. Status: {StatusCode}, Error: {Error}", 
                    id, response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar activo {Id} en la API", id);
                throw;
            }
        }

        public async Task<bool> EliminarActivoAsync(int id)
        {
            try
            {
                using var request = CreateRequestMessage(HttpMethod.Delete, $"/api/Activos/{id}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                _logger.LogWarning("Error al eliminar activo {Id}. Status: {StatusCode}", id, response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar activo {Id} en la API", id);
                throw;
            }
        }

        public async Task<bool> ExisteActivoAsync(int id)
        {
            try
            {
                var activo = await GetActivoPorIdAsync(id);
                return activo != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia del activo {Id}", id);
                return false;
            }
        }

        #region Métodos de Mapeo

        private ActivoFiltroDto MapFiltroToDto(ActivoFiltroViewModel? filtro)
        {
            if (filtro == null)
                return new ActivoFiltroDto();

            return new ActivoFiltroDto
            {
                IdActivo = filtro.IdActivo,
                CodigoLike = filtro.CodigoLike,
                MarcaLike = filtro.MarcaLike,
                NombreLike = filtro.NombreLike,
                PersonaAsignLike = filtro.PersonaAsignLike,
                UbicacionLike = filtro.UbicacionLike,
                TipoActivoId = filtro.TipoActivoId,
                DepartamentoId = filtro.DepartamentoId,
                StatusId = filtro.StatusId,
                ProveedorId = filtro.ProveedorId,
                TieneSoftwareOP = filtro.TieneSoftwareOP,
                CostoMin = filtro.CostoMin,
                CostoMax = filtro.CostoMax,
                FechaAltaDesde = filtro.FechaAltaDesde,
                FechaAltaHasta = filtro.FechaAltaHasta,
                FechaCompraDesde = filtro.FechaCompraDesde,
                FechaCompraHasta = filtro.FechaCompraHasta,
                FechaBajaDesde = filtro.FechaBajaDesde,
                FechaBajaHasta = filtro.FechaBajaHasta,
                ComponentesSeleccionados = filtro.ComponentesSeleccionados ?? new List<int>()
            };
        }

        private ActivoViewModel MapDtoToViewModel(ActivoDto dto)
        {
            return new ActivoViewModel
            {
                IdActivo = dto.IdActivo,
                Codigo = dto.Codigo,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Serie = dto.Serie,
                Nombre = dto.Nombre,
                PersonaAsign = dto.PersonaAsign,
                Ubicacion = dto.Ubicacion,
                FeCompra = dto.FeCompra,
                FeAlta = dto.FeAlta,
                FeBaja = dto.FeBaja,
                CostoCompra = dto.CostoCompra,
                Notas = dto.Notas,
                CodificacionComponentes = dto.CodificacionComponentes,
                TieneSoftwareOP = dto.TieneSoftwareOP,
                IdTipoActivo = dto.IdTipoActivo,
                IdDepartamento = dto.IdDepartamento,
                IdStatus = dto.IdStatus,
                IdProveedor = dto.IdProveedor,
                TipoActivoNombre = dto.TipoActivoNombre,
                DepartamentoNombre = dto.DepartamentoNombre,
                StatusNombre = dto.StatusNombre,
                ProveedorNombre = dto.ProveedorNombre
            };
        }

        private ActivoCreateDto MapViewModelToCreateDto(ActivoViewModel viewModel)
        {
            return new ActivoCreateDto
            {
                Codigo = viewModel.Codigo,
                Marca = viewModel.Marca,
                Modelo = viewModel.Modelo,
                Serie = viewModel.Serie,
                Nombre = viewModel.Nombre,
                PersonaAsign = viewModel.PersonaAsign,
                Ubicacion = viewModel.Ubicacion,
                FeCompra = viewModel.FeCompra,
                FeAlta = viewModel.FeAlta,
                FeBaja = viewModel.FeBaja,
                CostoCompra = viewModel.CostoCompra,
                Notas = viewModel.Notas,
                CodificacionComponentes = viewModel.CodificacionComponentes ?? 0,
                TieneSoftwareOP = viewModel.TieneSoftwareOP,
                IdTipoActivo = viewModel.IdTipoActivo,
                IdDepartamento = viewModel.IdDepartamento,
                IdStatus = viewModel.IdStatus,
                IdProveedor = viewModel.IdProveedor
            };
        }

        private ActivoUpdateDto MapViewModelToUpdateDto(ActivoViewModel viewModel)
        {
            var updateDto = new ActivoUpdateDto
            {
                IdActivo = viewModel.IdActivo,
                Codigo = viewModel.Codigo,
                Marca = viewModel.Marca,
                Modelo = viewModel.Modelo,
                Serie = viewModel.Serie,
                Nombre = viewModel.Nombre,
                PersonaAsign = viewModel.PersonaAsign,
                Ubicacion = viewModel.Ubicacion,
                FeCompra = viewModel.FeCompra,
                FeAlta = viewModel.FeAlta,
                FeBaja = viewModel.FeBaja,
                CostoCompra = viewModel.CostoCompra,
                Notas = viewModel.Notas,
                CodificacionComponentes = viewModel.CodificacionComponentes ?? 0,
                TieneSoftwareOP = viewModel.TieneSoftwareOP,
                IdTipoActivo = viewModel.IdTipoActivo,
                IdDepartamento = viewModel.IdDepartamento,
                IdStatus = viewModel.IdStatus,
                IdProveedor = viewModel.IdProveedor
            };
            return updateDto;
        }

        private string BuildQueryString(ActivoFiltroDto filtro)
        {
            var queryParams = new List<string>();

            if (filtro.IdActivo.HasValue)
                queryParams.Add($"IdActivo={filtro.IdActivo.Value}");

            if (!string.IsNullOrEmpty(filtro.CodigoLike))
                queryParams.Add($"CodigoLike={Uri.EscapeDataString(filtro.CodigoLike)}");

            if (!string.IsNullOrEmpty(filtro.MarcaLike))
                queryParams.Add($"MarcaLike={Uri.EscapeDataString(filtro.MarcaLike)}");

            if (!string.IsNullOrEmpty(filtro.NombreLike))
                queryParams.Add($"NombreLike={Uri.EscapeDataString(filtro.NombreLike)}");

            if (!string.IsNullOrEmpty(filtro.PersonaAsignLike))
                queryParams.Add($"PersonaAsignLike={Uri.EscapeDataString(filtro.PersonaAsignLike)}");

            if (!string.IsNullOrEmpty(filtro.UbicacionLike))
                queryParams.Add($"UbicacionLike={Uri.EscapeDataString(filtro.UbicacionLike)}");

            if (filtro.TipoActivoId.HasValue)
                queryParams.Add($"TipoActivoId={filtro.TipoActivoId.Value}");

            if (filtro.DepartamentoId.HasValue)
                queryParams.Add($"DepartamentoId={filtro.DepartamentoId.Value}");

            if (filtro.StatusId.HasValue)
                queryParams.Add($"StatusId={filtro.StatusId.Value}");

            if (filtro.ProveedorId.HasValue)
                queryParams.Add($"ProveedorId={filtro.ProveedorId.Value}");

            if (filtro.TieneSoftwareOP.HasValue)
                queryParams.Add($"TieneSoftwareOP={filtro.TieneSoftwareOP.Value}");

            if (filtro.CostoMin.HasValue)
                queryParams.Add($"CostoMin={filtro.CostoMin.Value}");

            if (filtro.CostoMax.HasValue)
                queryParams.Add($"CostoMax={filtro.CostoMax.Value}");

            if (filtro.FechaAltaDesde.HasValue)
                queryParams.Add($"FechaAltaDesde={filtro.FechaAltaDesde.Value:yyyy-MM-dd}");

            if (filtro.FechaAltaHasta.HasValue)
                queryParams.Add($"FechaAltaHasta={filtro.FechaAltaHasta.Value:yyyy-MM-dd}");

            if (filtro.FechaCompraDesde.HasValue)
                queryParams.Add($"FechaCompraDesde={filtro.FechaCompraDesde.Value:yyyy-MM-dd}");

            if (filtro.FechaCompraHasta.HasValue)
                queryParams.Add($"FechaCompraHasta={filtro.FechaCompraHasta.Value:yyyy-MM-dd}");

            if (filtro.FechaBajaDesde.HasValue)
                queryParams.Add($"FechaBajaDesde={filtro.FechaBajaDesde.Value:yyyy-MM-dd}");

            if (filtro.FechaBajaHasta.HasValue)
                queryParams.Add($"FechaBajaHasta={filtro.FechaBajaHasta.Value:yyyy-MM-dd}");

            if (filtro.ComponentesSeleccionados != null && filtro.ComponentesSeleccionados.Any())
            {
                foreach (var componente in filtro.ComponentesSeleccionados)
                {
                    queryParams.Add($"ComponentesSeleccionados={componente}");
                }
            }

            return queryParams.Any() ? "?" + string.Join("&", queryParams) : string.Empty;
        }

        #endregion
    }
}

