using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoAssits.Core.Dtos
{
    public class UsuarioResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public byte? IdDepartamento { get; set; }
        public string? DepartamentoNombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public int TicketsSolucionados { get; set; }
        public int TicketsEnProceso { get; set; }
        public int TicketsPendientes { get; set; }
        public TimeSpan? TiempoPromedioResolucion { get; set; }
    }

    public class UsuarioUpdateDto
    {
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(150, ErrorMessage = "El nombre completo no puede exceder 150 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(50, ErrorMessage = "El email no puede exceder 50 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        public string Usuario { get; set; } = string.Empty;

        public byte? IdDepartamento { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class UsuarioRolDto
    {
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public string UsuarioId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es requerido")]
        public string Rol { get; set; } = string.Empty;
    }

    public class UsuarioRolesDto
    {
        [Required(ErrorMessage = "El ID del usuario es requerido")]
        public string UsuarioId { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UsuarioFiltroDto
    {
        public string? BusquedaTexto { get; set; }
        public byte? IdDepartamento { get; set; }
        public bool? Activo { get; set; }
        public string? Rol { get; set; }
        public DateTime? FechaRegistroInicio { get; set; }
        public DateTime? FechaRegistroFin { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
    }

    public class UsuarioPaginadoDto
    {
        public List<UsuarioResponseDto> Usuarios { get; set; } = new List<UsuarioResponseDto>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamañoPagina { get; set; }
    }

    public class UsuarioEstadisticasDto
    {
        public int TotalUsuarios { get; set; }
        public int UsuariosActivos { get; set; }
        public int UsuariosInactivos { get; set; }
        public Dictionary<string, int> PorDepartamento { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PorRol { get; set; } = new Dictionary<string, int>();
        public int UsuariosNuevosEsteMes { get; set; }
        public int UsuariosConActividadReciente { get; set; }
    }

    public class UsuarioPerfilDto
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string? DepartamentoNombre { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public int TicketsSolucionados { get; set; }
        public int TicketsEnProceso { get; set; }
        public int TicketsPendientes { get; set; }
        public TimeSpan? TiempoPromedioResolucion { get; set; }
    }

}
