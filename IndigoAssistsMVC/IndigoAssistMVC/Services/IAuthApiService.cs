namespace IndigoAssistMVC.Services
{
    /// <summary>
    /// Interfaz para el servicio de autenticación con la API
    /// </summary>
    public interface IAuthApiService
    {
        /// <summary>
        /// Realiza login en la API y obtiene el token JWT
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>Token JWT y fecha de expiración, o null si las credenciales son inválidas</returns>
        Task<AuthTokenResponse?> LoginAsync(string userName, string password);
    }

    /// <summary>
    /// Respuesta del login con token JWT
    /// </summary>
    public class AuthTokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
}

