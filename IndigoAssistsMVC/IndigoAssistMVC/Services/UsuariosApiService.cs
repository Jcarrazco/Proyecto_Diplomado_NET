using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IndigoAssistMVC.Services
{
    public class UsuariosApiService : IUsuariosApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsuariosApiService> _logger;
        private readonly string _apiBaseUrl;
        private readonly string _sucursal;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuariosApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<UsuariosApiService> logger,
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

        public async Task<bool> EnsureLegacyUserAsync(string email, string legacyLogin, byte? idDepto)
        {
            try
            {
                var payload = new
                {
                    Email = email,
                    Login = legacyLogin,
                    IdDepto = idDepto
                };

                using var request = CreateRequestMessage(HttpMethod.Post, "/api/Usuarios/legacy/ensure");
                request.Content = JsonContent.Create(payload);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al crear usuario legacy. Status: {StatusCode}, Response: {Error}", response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario legacy desde la API");
                return false;
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
                DateTime.TryParse(expiresAtRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAt))
            {
                if (expiresAt <= DateTime.UtcNow)
                {
                    httpContext.Session.Remove("ApiToken");
                    httpContext.Session.Remove("ApiTokenExpiresAt");
                    return null;
                }
            }

            return token;
        }
    }
}
