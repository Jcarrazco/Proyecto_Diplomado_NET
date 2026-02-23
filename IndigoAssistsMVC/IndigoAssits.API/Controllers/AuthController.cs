using IndigoAssits.Core.Dtos;
using IndigoAssitsReglasDeNegocio.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace IndigoAssits.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUsuarioService usuarioService, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
            _logger = logger;
        }

        public class LoginRequest
        {
            [System.Text.Json.Serialization.JsonPropertyName("userName")]
            [Newtonsoft.Json.JsonProperty("userName")]
            public string UserName { get; set; } = string.Empty;
            
            [System.Text.Json.Serialization.JsonPropertyName("password")]
            [Newtonsoft.Json.JsonProperty("password")]
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public string AccessToken { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest? request)
        {
            try
            {
                // Validar que el request no sea null
                if (request == null)
                {
                    _logger.LogWarning("Request de login es null");
                    return BadRequest(new { error = "Request inválido", message = "El cuerpo de la petición no puede estar vacío" });
                }

                _logger.LogInformation("Intento de login para usuario: {UserName}", request.UserName ?? "null");

                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Credenciales vacías en request");
                    return BadRequest(new { error = "Credenciales inválidas", message = "UserName y Password son requeridos" });
                }

                _logger.LogDebug("Validando credenciales para usuario: {UserName}", request.UserName);
                bool valido;
                try
                {
                    valido = await _usuarioService.ValidarCredencialesAsync(request.UserName, request.Password);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al validar credenciales para usuario: {UserName}", request.UserName);
                    return StatusCode(500, new { error = "Error al validar credenciales", message = ex.Message });
                }

                if (!valido)
                {
                    _logger.LogWarning("Credenciales inválidas para usuario: {UserName}", request.UserName);
                    return Unauthorized(new { error = "Credenciales inválidas", message = "El usuario o contraseña son incorrectos" });
                }

                _logger.LogDebug("Obteniendo información del usuario: {UserName}", request.UserName);
                UsuarioResponseDto? user;
                try
                {
                    user = await _usuarioService.GetPorUserNameAsync(request.UserName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener usuario: {UserName}", request.UserName);
                    return StatusCode(500, new { error = "Error al obtener usuario", message = ex.Message });
                }

                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado: {UserName}", request.UserName);
                    return Unauthorized(new { error = "Usuario no encontrado", message = $"El usuario {request.UserName} no existe" });
                }

                _logger.LogDebug("Obteniendo roles para usuario: {UserId}", user.Id);
                IEnumerable<string> roles;
                try
                {
                    roles = await _usuarioService.GetRolesAsync(user.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener roles para usuario: {UserId}", user.Id);
                    return StatusCode(500, new { error = "Error al obtener roles", message = ex.Message });
                }
                _logger.LogDebug("Roles obtenidos: {Roles}", string.Join(", ", roles));

                var jwtSection = _configuration.GetSection("Jwt");
                if (string.IsNullOrEmpty(jwtSection["Key"]))
                {
                    return StatusCode(500, "Error de configuración: JWT Key no configurada");
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.UtcNow.AddMinutes(int.TryParse(jwtSection["AccessTokenMinutes"], out var m) ? m : 60);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Usuario ?? request.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
                };
                claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

                var token = new JwtSecurityToken(
                    issuer: jwtSection["Issuer"],
                    audience: jwtSection["Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: creds);

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(new LoginResponse { AccessToken = accessToken, ExpiresAt = expires });
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                var userName = request?.UserName ?? "desconocido";
                _logger.LogError(ex, "Error al procesar login para usuario: {UserName}. Error: {Error}, StackTrace: {StackTrace}", 
                    userName, ex.Message, ex.StackTrace);
                
                // Devolver error en formato JSON que se pueda serializar
                var errorResponse = new
                {
                    error = "Error interno del servidor",
                    message = ex.Message,
                    innerException = ex.InnerException?.Message
                };
                
                return StatusCode(500, errorResponse);
            }
        }
    }
}


