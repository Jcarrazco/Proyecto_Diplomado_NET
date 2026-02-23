using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoAssits.Core.Dtos
{
    public class TicketCreateDto : IValidatableObject
    {
        public int UsuarioSolicitante { get; set; }

        public string? UsuarioSolicitanteLogin { get; set; }

        public byte IdSubCategoria { get; set; }

        public string? SubCategoriaNombre { get; set; }

        public string? CategoriaNombre { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(50, ErrorMessage = "El título no puede exceder 50 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        public byte IdTipoTicket { get; set; }

        public string? TipoTicketNombre { get; set; }

        public byte Prioridad { get; set; }

        public string? PrioridadNombre { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UsuarioSolicitante <= 0 && string.IsNullOrWhiteSpace(UsuarioSolicitanteLogin))
            {
                yield return new ValidationResult(
                    "El usuario solicitante es requerido (ID o login).",
                    new[] { nameof(UsuarioSolicitante), nameof(UsuarioSolicitanteLogin) });
            }

            if (IdSubCategoria <= 0 && string.IsNullOrWhiteSpace(SubCategoriaNombre))
            {
                yield return new ValidationResult(
                    "La subcategoría es requerida (ID o nombre).",
                    new[] { nameof(IdSubCategoria), nameof(SubCategoriaNombre) });
            }
        }
    }

    public class TicketUpdateDto
    {
        [Required(ErrorMessage = "El ID del ticket es requerido")]
        public int IdTicket { get; set; }

        [StringLength(50, ErrorMessage = "El título no puede exceder 50 caracteres")]
        public string? Titulo { get; set; }

        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        public string? Descripcion { get; set; }

        public byte? Status { get; set; }

        public byte? IdTipoTicket { get; set; }

        public byte? Prioridad { get; set; }

        public byte? IdSubCategoria { get; set; }

        public DateTime? FeAsignacion { get; set; }

        public DateTime? FeCompromiso { get; set; }

        public DateTime? FeCierre { get; set; }
    }

    public class TicketAsignacionDto
    {
        [Required(ErrorMessage = "El ID del ticket es requerido")]
        public int IdTicket { get; set; }

        [Required(ErrorMessage = "El ID del técnico es requerido")]
        public int IdTecnico { get; set; }

        public DateTime FeCompromiso { get; set; }
    }

    public class TicketAsignacionMultipleDto
    {
        [Required(ErrorMessage = "El ID del ticket es requerido")]
        public int IdTicket { get; set; }

        [Required(ErrorMessage = "Debe especificar al menos un técnico")]
        public List<int> Tecnicos { get; set; } = new List<int>();

        public DateTime? FeCompromiso { get; set; }
    }

    public class TicketAnotacionCreateDto
    {
        [Required(ErrorMessage = "El ID del ticket es requerido")]
        public int IdTicket { get; set; }

        [Required(ErrorMessage = "La anotación es requerida")]
        [StringLength(2000, ErrorMessage = "La anotación no puede exceder 2000 caracteres")]
        public string Observacion { get; set; } = string.Empty;

        public int Duracion { get; set; }

        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50)]
        public string Usuario { get; set; } = string.Empty;
    }

    public class TicketResponseDto
    {
        public int IdTicket { get; set; }
        public int UsuarioSolicitante { get; set; }
        public string SolicitanteNombre { get; set; } = string.Empty;
        public byte IdSubCategoria { get; set; }
        public string SubCategoriaNombre { get; set; } = string.Empty;
        public byte IdCategoria { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string StatusDescripcion { get; set; } = string.Empty;
        public byte IdTipoTicket { get; set; }
        public string TipoTicketNombre { get; set; } = string.Empty;
        public byte Prioridad { get; set; }
        public string PrioridadNombre { get; set; } = string.Empty;
        public DateTime FeAlta { get; set; }
        public DateTime FeAsignacion { get; set; }
        public DateTime FeCompromiso { get; set; }
        public DateTime FeCierre { get; set; }
        public int IdTecnico { get; set; }
        public string TecnicoNombre { get; set; } = string.Empty;
        public byte IdDepartamento { get; set; }
        public string DepartamentoNombre { get; set; } = string.Empty;
    }

    public class TicketFiltroDto
    {
        public int UsuarioSolicitante { get; set; }
        public int IdTecnico { get; set; }
        public string? Status { get; set; }
        public byte? StatusId { get; set; }
        public byte IdCategoria { get; set; }
        public byte IdSubCategoria { get; set; }
        public byte Prioridad { get; set; }
        public byte IdTipoTicket { get; set; }
        public byte IdDepartamento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? BusquedaTexto { get; set; }
        public string? Titulo { get; set; }
        public string? Descripcion { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
    }

    public class TicketEstadisticasDto
    {
        public int TotalAbiertos { get; set; }
        public int TotalEnProceso { get; set; }
        public int TotalAsignados { get; set; }
        public int TotalCerrados { get; set; }
        public Dictionary<string, int> PorDepartamento { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PorPrioridad { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PorEstado { get; set; } = new Dictionary<string, int>();
    }

    public class TicketPaginadoDto
    {
        public List<TicketResponseDto> Tickets { get; set; } = new List<TicketResponseDto>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamañoPagina { get; set; }
    }
}
