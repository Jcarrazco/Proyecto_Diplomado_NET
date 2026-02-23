using IndigoAssits.Core.Dtos;

namespace IndigoAssitsReglasDeNegocio.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDto?> GetPorUserNameAsync(string userName);
        Task<bool> ValidarCredencialesAsync(string userName, string password);
        Task<IEnumerable<string>> GetRolesAsync(string userId);
    }
}

