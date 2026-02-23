using System.Net.Http.Json;
using IndigoAssistMVC.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IndigoAssistMVC.Services
{
    /// <summary>
    /// Servicio para autenticación con la API y obtención de token JWT
    /// </summary>
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiService> _logger;
        private readonly string _apiBaseUrl;

        public AuthApiService(HttpClient httpClient, IConfiguration configuration, ILogger<AuthApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5124";
            
            // Configurar la URL base del HttpClient
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<AuthTokenResponse?> LoginAsync(string userName, string password)
        {
            try
            {
                // El AuthController espera UserName (con mayúscula) y Password
                var loginRequest = new
                {
                    UserName = userName,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (loginResponse != null)
                    {
                        return new AuthTokenResponse
                        {
                            AccessToken = loginResponse.AccessToken,
                            ExpiresAt = loginResponse.ExpiresAt
                        };
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Credenciales inválidas para el usuario: {UserName}", userName);
                    return null;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error al hacer login. Status: {StatusCode}, Error: {Error}, Request: UserName={UserName}", 
                        response.StatusCode, errorContent, userName);
                    
                    // Intentar parsear el error como JSON para obtener más detalles
                    try
                    {
                        var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                        if (errorObj != null && errorObj.ContainsKey("message"))
                        {
                            _logger.LogError("Detalle del error: {Message}", errorObj["message"]);
                        }
                    }
                    catch
                    {
                        // Si no es JSON, usar el contenido tal cual
                    }
                    
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al hacer login en la API para el usuario: {UserName}", userName);
                return null;
            }

            return null;
        }

        /// <summary>
        /// DTO para la respuesta del login de la API
        /// </summary>
        private class LoginResponseDto
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
        }
    }
}

