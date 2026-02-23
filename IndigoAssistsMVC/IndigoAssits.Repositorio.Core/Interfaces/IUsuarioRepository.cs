using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{

    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<Usuario?> GetUsuarioByEmailAsync(string email);
        Task<Usuario?> GetUsuarioByUserNameAsync(string userName);
        Task<IEnumerable<Usuario>> GetUsuariosByDepartamentoAsync(byte departamentoId);
        Task<IEnumerable<Usuario>> GetUsuariosByRolAsync(string rol);

        Task<Usuario?> ValidateCredentialsAsync(string userName, string password);
        Task<bool> ChangePasswordAsync(string userId, string newPasswordHash);

        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<bool> AddUserToRoleAsync(string userId, string role);
        Task<bool> RemoveUserFromRoleAsync(string userId, string role);
        Task<bool> IsUserInRoleAsync(string userId, string role);

        Task<Dictionary<string, int>> GetEstadisticasUsuariosAsync();

        Task<bool> ExisteUsuarioAsync(string userName);
        Task<bool> ExisteEmailAsync(string email);
        Task<bool> IsUsuarioActivoAsync(string userId);
    }
}
