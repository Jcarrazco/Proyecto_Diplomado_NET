using IndigoAssits.Core.Dtos;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAssitsReglasDeNegocio.Interfaces;

namespace IndigoAssitsReglasDeNegocio.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioResponseDto?> GetPorUserNameAsync(string userName)
        {
            var u = await _unitOfWork.Usuarios.GetUsuarioByUserNameAsync(userName);
            if (u == null) return null;
            var roles = await _unitOfWork.Usuarios.GetUserRolesAsync(u.Id);
            return new UsuarioResponseDto
            {
                Id = u.Id,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email ?? string.Empty,
                Usuario = u.UserName ?? string.Empty,
                IdDepartamento = u.IdDepartamento,
                Activo = u.Activo,
                FechaRegistro = u.FechaRegistro,
                UltimoAcceso = u.UltimoAcceso,
                Roles = roles.ToList()
            };
        }

        public async Task<bool> ValidarCredencialesAsync(string userName, string password)
        {
            var u = await _unitOfWork.Usuarios.ValidateCredentialsAsync(userName, password);
            return u != null;
        }

        public async Task<IEnumerable<string>> GetRolesAsync(string userId)
        {
            return await _unitOfWork.Usuarios.GetUserRolesAsync(userId);
        }
    }
}


