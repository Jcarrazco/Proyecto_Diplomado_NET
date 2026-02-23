using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IndigoAssistMVC.ViewModels
{
    /// <summary>
    /// ViewModel para la gestión completa de usuarios
    /// </summary>
    public class UsuarioGestionViewModel
    {
        public List<UsuarioViewModel> Usuarios { get; set; } = new List<UsuarioViewModel>();
        public UsuarioFiltroViewModel Filtros { get; set; } = new UsuarioFiltroViewModel();
        public int TotalUsuarios { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; } = 10;
    }

    /// <summary>
    /// ViewModel para mostrar información de usuario
    /// </summary>
    public class UsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;
        
        [Display(Name = "Departamento")]
        public string? DepartamentoNombre { get; set; }
        
        [Display(Name = "Activo")]
        public bool Activo { get; set; }
        
        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; }
        
        [Display(Name = "Último Acceso")]
        public DateTime? UltimoAcceso { get; set; }
        
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; } = new List<string>();
        
        [Display(Name = "Email Confirmado")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Bloqueado")]
        public bool IsLockedOut { get; set; }

        [Display(Name = "Fin Bloqueo")]
        public DateTimeOffset? LockoutEnd { get; set; }
    }

    /// <summary>
    /// ViewModel para crear un nuevo usuario
    /// </summary>
    public class CrearUsuarioViewModel
    {
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder 50 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(150, ErrorMessage = "El nombre completo no puede exceder 150 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Departamento")]
        public byte? IdDepartamento { get; set; }

        [Display(Name = "Login Legacy")]
        [StringLength(12, ErrorMessage = "El login legacy no puede exceder 12 caracteres")]
        public string? LegacyLogin { get; set; }

        [Display(Name = "Roles")]
        public List<string> RolesSeleccionados { get; set; } = new List<string>();

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Lista de departamentos disponibles
        public List<DepartamentoViewModel> DepartamentosDisponibles { get; set; } = new List<DepartamentoViewModel>();
        
        // Lista de roles disponibles
        public List<RolViewModel> RolesDisponibles { get; set; } = new List<RolViewModel>();
    }

    /// <summary>
    /// ViewModel para editar un usuario existente
    /// </summary>
    public class EditarUsuarioViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder 50 caracteres")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(150, ErrorMessage = "El nombre completo no puede exceder 150 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Departamento")]
        public byte? IdDepartamento { get; set; }

        [Display(Name = "Roles")]
        public List<string> RolesSeleccionados { get; set; } = new List<string>();

        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [Display(Name = "Email Confirmado")]
        public bool EmailConfirmed { get; set; }

        // Lista de departamentos disponibles
        public List<DepartamentoViewModel> DepartamentosDisponibles { get; set; } = new List<DepartamentoViewModel>();
        
        // Lista de roles disponibles
        public List<RolViewModel> RolesDisponibles { get; set; } = new List<RolViewModel>();
    }

    /// <summary>
    /// ViewModel para cambio de contraseña
    /// </summary>
    public class CambiarPasswordViewModel
    {
        public string UsuarioId { get; set; } = string.Empty;
        
        [Display(Name = "Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Nueva Contraseña")]
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        [DataType(DataType.Password)]
        public string NuevaPassword { get; set; } = string.Empty;

        [Display(Name = "Confirmar Nueva Contraseña")]
        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        [DataType(DataType.Password)]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Display(Name = "Forzar cambio en próximo login")]
        public bool ForzarCambioEnProximoLogin { get; set; } = true;
    }

    /// <summary>
    /// ViewModel para filtros de búsqueda de usuarios
    /// </summary>
    public class UsuarioFiltroViewModel
    {
        [Display(Name = "Buscar")]
        public string? Buscar { get; set; }

        [Display(Name = "Rol")]
        public string? RolFiltro { get; set; }

        [Display(Name = "Departamento")]
        public byte? IdDepartamentoFiltro { get; set; }

        [Display(Name = "Estado")]
        public bool? ActivoFiltro { get; set; }

        [Display(Name = "Email Confirmado")]
        public bool? EmailConfirmedFiltro { get; set; }

        // Lista de roles disponibles para filtro
        public List<RolViewModel> RolesDisponibles { get; set; } = new List<RolViewModel>();
        
        // Lista de departamentos disponibles para filtro
        public List<DepartamentoViewModel> DepartamentosDisponibles { get; set; } = new List<DepartamentoViewModel>();
    }

    /// <summary>
    /// ViewModel para roles
    /// </summary>
    public class RolViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Seleccionado { get; set; }
    }

    /// <summary>
    /// ViewModel para departamentos
    /// </summary>
    public class DepartamentoViewModel
    {
        public byte IdDepartamento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionado { get; set; }
    }

    /// <summary>
    /// ViewModel para el dashboard de usuarios con filtros
    /// </summary>
    public class UsuarioDashboardViewModel
    {
        public UsuarioFiltroViewModel Filtros { get; set; } = new UsuarioFiltroViewModel();
        public List<UsuarioViewModel> Usuarios { get; set; } = new List<UsuarioViewModel>();
        public Dictionary<string, int> EstadisticasGenerales { get; set; } = new Dictionary<string, int>();
        
        [Display(Name = "Total Usuarios")]
        public int TotalUsuarios { get; set; }
        
        [Display(Name = "Usuarios Activos")]
        public int UsuariosActivos { get; set; }
        
        [Display(Name = "Usuarios Inactivos")]
        public int UsuariosInactivos { get; set; }
        
        [Display(Name = "Usuarios por Rol")]
        public Dictionary<string, int> UsuariosPorRol { get; set; } = new Dictionary<string, int>();
    }
}
