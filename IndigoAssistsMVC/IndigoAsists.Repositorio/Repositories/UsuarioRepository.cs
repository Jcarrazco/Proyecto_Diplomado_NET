using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAsists.Repositorio.Db;

namespace IndigoAsists.Repositorio.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// </summary>
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly UserManager<Usuario>? _userManager;

        public UsuarioRepository(IndigoDbContext context, UserManager<Usuario>? userManager) 
            : base(context)
        {
            _userManager = userManager;
        }

        public async Task<Usuario?> GetUsuarioByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetUsuarioByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;

            if (_userManager != null)
            {
                // Intentar por UserName (Identity usa NormalizedUserName internamente)
                var byName = await _userManager.FindByNameAsync(userName);
                if (byName != null) return byName;

                // Fallback: intentar por Email
                var byEmail = await _userManager.FindByEmailAsync(userName);
                if (byEmail != null) return byEmail;

                return null;
            }

            // Fallback sin UserManager: comparar usando valores normalizados
            var normalized = userName.ToUpperInvariant();
            return await _dbSet.FirstOrDefaultAsync(u =>
                u.NormalizedUserName == normalized ||
                u.Email == userName ||
                u.NormalizedEmail == normalized);
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosByDepartamentoAsync(byte departamentoId)
        {
            return await _dbSet
                .Where(u => u.IdDepartamento == departamentoId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosByRolAsync(string rol)
        {
            if (_userManager == null) return new List<Usuario>();
            var usuariosEnRol = await _userManager.GetUsersInRoleAsync(rol);
            return usuariosEnRol.AsEnumerable();
        }

        public async Task<Usuario?> ValidateCredentialsAsync(string userName, string password)
        {
            var usuario = await GetUsuarioByUserNameAsync(userName);
            if (usuario == null || _userManager == null) return null;

            var result = await _userManager.CheckPasswordAsync(usuario, password);
            return result ? usuario : null;
        }

        public async Task<bool> ChangePasswordAsync(string userId, string newPasswordHash)
        {
            if (_userManager == null) return false;
            
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var result = await _userManager.ResetPasswordAsync(usuario, token, newPasswordHash);
            
            return result.Succeeded;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            if (_userManager == null) return new List<string>();
            
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return new List<string>();
            
            var roles = await _userManager.GetRolesAsync(usuario);
            return roles;
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string role)
        {
            if (_userManager == null) return false;
            
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            var result = await _userManager.AddToRoleAsync(usuario, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string role)
        {
            if (_userManager == null) return false;
            
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            var result = await _userManager.RemoveFromRoleAsync(usuario, role);
            return result.Succeeded;
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string role)
        {
            if (_userManager == null) return false;
            
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            return await _userManager.IsInRoleAsync(usuario, role);
        }

        public async Task<Dictionary<string, int>> GetEstadisticasUsuariosAsync()
        {
            var estadisticas = new Dictionary<string, int>();

            // Total de usuarios
            estadisticas["TotalUsuarios"] = await _dbSet.CountAsync();

            // Usuarios activos
            estadisticas["UsuariosActivos"] = await _dbSet.CountAsync(u => u.Activo);

            // Usuarios por departamento
            var usuariosPorDepto = await _dbSet
                .Where(u => u.IdDepartamento.HasValue)
                .GroupBy(u => u.IdDepartamento)
                .ToDictionaryAsync(g => $"Depto_{g.Key}", g => g.Count());

            foreach (var item in usuariosPorDepto)
            {
                estadisticas[item.Key] = item.Value;
            }

            return estadisticas;
        }

        public async Task<bool> ExisteUsuarioAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;

            if (_userManager != null)
            {
                var byName = await _userManager.FindByNameAsync(userName);
                if (byName != null) return true;

                var byEmail = await _userManager.FindByEmailAsync(userName);
                return byEmail != null;
            }

            var normalized = userName.ToUpperInvariant();
            return await _dbSet.AnyAsync(u =>
                u.NormalizedUserName == normalized ||
                u.Email == userName ||
                u.NormalizedEmail == normalized);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsuarioActivoAsync(string userId)
        {
            var usuario = await _dbSet.FindAsync(userId);
            return usuario?.Activo ?? false;
        }
    }
}