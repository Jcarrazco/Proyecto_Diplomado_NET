using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigoAssits.Repositorio.Core.Entities
{
    [Table("mTickets")]
    public class Ticket
    {
        [Key]
        [Display(Name = "ID Ticket")]
        public int IdTicket { get; set; }

        [Display(Name = "Usuario Solicitante")]
        [Required]
        public int Usuario { get; set; }

        [Display(Name = "Subcategoría")]
        [Required]
        public byte IdSubCategoria { get; set; }

        [Display(Name = "Título")]
        [Required, StringLength(50)]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        [Required]
        public byte Status { get; set; }

        [Display(Name = "Tipo de Ticket")]
        public byte? IdTipoTicket { get; set; }

        [Display(Name = "Prioridad")]
        public byte? Prioridad { get; set; }

        [Display(Name = "Fecha de Alta")]
        [Required]
        public DateTime FeAlta { get; set; }

        [Display(Name = "Fecha de Asignación")]
        public DateTime? FeAsignacion { get; set; }

        [Display(Name = "Fecha de Compromiso")]
        public DateTime? FeCompromiso { get; set; }

        [Display(Name = "Fecha de Cierre")]
        public DateTime? FeCierre { get; set; }

        // Propiedades de navegación
        [ForeignKey("Usuario")]
        public virtual mPerEmp? UsuarioSolicitante { get; set; }

        [ForeignKey("IdSubCategoria")]
        public virtual mSubCategoriasTicket? SubCategoria { get; set; }

        [ForeignKey("Status")]
        public virtual mStatusTicket? Estado { get; set; }

        [ForeignKey("IdTipoTicket")]
        public virtual mTipoTicket? TipoTicket { get; set; }

        [ForeignKey("Prioridad")]
        public virtual mPrioridadTicket? PrioridadTicket { get; set; }
    }

    [Table("vTickets")]
    public class TicketVista
    {
        [Key]
        [Display(Name = "ID Ticket")]
        public int IdTicket { get; set; }

        [Display(Name = "ID Solicitante")]
        public int IdSolicitante { get; set; }

        [Display(Name = "Solicitante")]
        public string Solicitante { get; set; } = string.Empty;

        [Display(Name = "ID Técnico")]
        public int? IdTecnico { get; set; }

        [Display(Name = "Técnico")]
        public string Tecnico { get; set; } = string.Empty;

        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public byte Status { get; set; }

        [Display(Name = "Estado Descripción")]
        public string StatusDes { get; set; } = string.Empty;

        [Display(Name = "Tipo de Ticket")]
        public byte? IdTipoTicket { get; set; }

        [Display(Name = "Prioridad")]
        public byte? IdPrioridad { get; set; }

        [Display(Name = "Fecha de Alta")]
        public DateTime FeAlta { get; set; }

        [Display(Name = "Fecha de Asignación")]
        public DateTime? FeAsignacion { get; set; }

        [Display(Name = "Fecha de Compromiso")]
        public DateTime? FeCompromiso { get; set; }

        [Display(Name = "Fecha de Cierre")]
        public DateTime? FeCierre { get; set; }

        [Display(Name = "ID Subcategoría")]
        public byte IdSubCategoria { get; set; }

        [Display(Name = "Subcategoría")]
        public string SubCategoria { get; set; } = string.Empty;

        [Display(Name = "ID Categoría")]
        public byte IdCategoria { get; set; }

        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = string.Empty;

        [Display(Name = "ID Departamento")]
        public byte IdDepto { get; set; }

        [Display(Name = "Departamento")]
        public string Departamento { get; set; } = string.Empty;
    }

    public class TicketDashboardViewModel
    {
        [Display(Name = "Usuario")]
        public mPerEmp Usuario { get; set; } = new mPerEmp();

        [Display(Name = "Tickets Abiertos por Departamento")]
        public List<TicketVista> TicketsAbiertosDepto { get; set; } = new List<TicketVista>();

        [Display(Name = "Tickets en Proceso por Departamento")]
        public List<TicketVista> TicketsEnProcesoDepto { get; set; } = new List<TicketVista>();

        [Display(Name = "Tickets en Proceso Asignados")]
        public List<TicketVista> TicketsEnProcesoAsignados { get; set; } = new List<TicketVista>();

        [Display(Name = "Tickets Cerrados por Departamento")]
        public List<TicketVista> TicketsCerradosDepto { get; set; } = new List<TicketVista>();

        [Display(Name = "Tickets Cerrados por Usuario")]
        public List<TicketVista> TicketsCerradosUsuario { get; set; } = new List<TicketVista>();

        [Display(Name = "Estadísticas")]
        public TicketEstadisticasViewModel Estadisticas { get; set; } = new TicketEstadisticasViewModel();
    }

    public class TicketEstadisticasViewModel
    {
        [Display(Name = "Total Tickets Abiertos")]
        public int TotalAbiertos { get; set; }

        [Display(Name = "Total Tickets en Proceso")]
        public int TotalEnProceso { get; set; }

        [Display(Name = "Total Tickets Asignados")]
        public int TotalAsignados { get; set; }

        [Display(Name = "Total Tickets Cerrados")]
        public int TotalCerrados { get; set; }

        [Display(Name = "Tickets por Departamento")]
        public Dictionary<string, int> PorDepartamento { get; set; } = new Dictionary<string, int>();

        [Display(Name = "Tickets por Prioridad")]
        public Dictionary<string, int> PorPrioridad { get; set; } = new Dictionary<string, int>();
    }
}
