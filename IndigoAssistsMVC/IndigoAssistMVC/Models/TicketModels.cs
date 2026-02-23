using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigoAssistMVC.Models
{
    /// <summary>
    /// Modelo para la tabla mTickets - Tickets principales
    /// </summary>
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

    /// <summary>
    /// Modelo para la vista vTickets - Vista completa de tickets
    /// </summary>
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

    /// <summary>
    /// Modelo para la tabla mPerEmp - Personas de la empresa
    /// </summary>
    [Table("mPerEmp")]
    public class mPerEmp
    {
        [Key]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        [Display(Name = "ID Empresa")]
        [Required]
        public byte IdEmpresa { get; set; }

        [Display(Name = "Persona")]
        [Required]
        public int Persona { get; set; }

        // Propiedades de navegación
        [ForeignKey("Persona")]
        public virtual mPersonas? PersonaInfo { get; set; }

        [ForeignKey("IdEmpresa")]
        public virtual mEmpresas? Empresa { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla mPersonas - Información de personas
    /// </summary>
    [Table("mPersonas")]
    public class mPersonas
    {
        [Key]
        [Display(Name = "Persona")]
        public int Persona { get; set; }

        [Display(Name = "Nombre")]
        [Required, StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Apellido Paterno")]
        [Required, StringLength(25)]
        public string Paterno { get; set; } = string.Empty;

        [Display(Name = "Apellido Materno")]
        [Required, StringLength(25)]
        public string Materno { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(50)]
        public string? Descripcion { get; set; }

        [Display(Name = "RFC")]
        [Required, StringLength(13)]
        public string RFC { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [Required, StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Tipo Persona")]
        [Required, StringLength(1)]
        public string TipoPersona { get; set; } = string.Empty;

        [Display(Name = "ID Régimen Fiscal")]
        public byte? IdRegimenFiscal { get; set; }

        [Display(Name = "ID Uso CFDI")]
        public byte? IdUsoCFDI { get; set; }

        [Display(Name = "ID Referencia")]
        public int? IdReferencia { get; set; }

        [Display(Name = "Usuario")]
        [Required, StringLength(12)]
        public string Usuario { get; set; } = string.Empty;

        [Display(Name = "Fecha Modificación")]
        public DateTime? FeModifica { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdReferencia")]
        public virtual mPersonas? ReferenciaNavigation { get; set; }

        // Propiedad calculada
        [NotMapped]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{Nombre} {Paterno} {Materno}".Trim();
    }

    /// <summary>
    /// Modelo para la tabla mEmpleados - Empleados del sistema
    /// </summary>
    [Table("mEmpleados")]
    public class mEmpleados
    {
        [Key]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        [Display(Name = "Login")]
        [StringLength(12)]
        public string? Login { get; set; }

        [Display(Name = "Activo")]
        [Required]
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        [ForeignKey("IdPersona")]
        public virtual mPerEmp? PersonaEmpresa { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla mDepartamentos - Departamentos
    /// </summary>
    [Table("mDepartamentos")]
    public class mDepartamentos
    {
        [Key]
        [Display(Name = "ID Departamento")]
        public byte IdDepto { get; set; }

        [Display(Name = "Departamento")]
        [Required, StringLength(50)]
        public string Departamento { get; set; } = string.Empty;

        [Display(Name = "Tickets")]
        [Required]
        public bool Tickets { get; set; } = false;
    }

    /// <summary>
    /// Modelo para la tabla mStatusTicket - Estados de tickets
    /// </summary>
    [Table("mStatusTicket")]
    public class mStatusTicket
    {
        [Key]
        [Display(Name = "Status")]
        public byte Status { get; set; }

        [Display(Name = "Descripción")]
        [Required, StringLength(10)]
        public string StatusDes { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo para la tabla mPrioridadTicket - Prioridades de tickets
    /// </summary>
    [Table("mPrioridadTicket")]
    public class mPrioridadTicket
    {
        [Key]
        [Display(Name = "ID Prioridad")]
        public byte IdPrioridad { get; set; }

        [Display(Name = "Prioridad")]
        [Required, StringLength(10)]
        public string Prioridad { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo para la tabla mTipoTicket - Tipos de tickets
    /// </summary>
    [Table("mTipoTicket")]
    public class mTipoTicket
    {
        [Key]
        [Display(Name = "ID Tipo Ticket")]
        public byte IdTipoTicket { get; set; }

        [Display(Name = "Tipo Ticket")]
        [Required, StringLength(20)]
        public string TipoTicket { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modelo para la tabla mCategoriasTicket - Categorías de tickets
    /// </summary>
    [Table("mCategoriasTicket")]
    public class mCategoriasTicket
    {
        [Key]
        [Display(Name = "ID Categoría")]
        public byte IdCategoria { get; set; }

        [Display(Name = "Categoría")]
        [Required, StringLength(30)]
        public string Categoria { get; set; } = string.Empty;

        [Display(Name = "ID Departamento")]
        [Required]
        public byte IdDepto { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdDepto")]
        public virtual mDepartamentos? Departamento { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla mSubCategoriasTicket - Subcategorías de tickets
    /// </summary>
    [Table("mSubCategoriasTicket")]
    public class mSubCategoriasTicket
    {
        [Key]
        [Display(Name = "ID Subcategoría")]
        public byte IdSubCategoria { get; set; }

        [Display(Name = "Subcategoría")]
        [Required, StringLength(30)]
        public string SubCategoria { get; set; } = string.Empty;

        [Display(Name = "ID Categoría")]
        [Required]
        public byte IdCategoria { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdCategoria")]
        public virtual mCategoriasTicket? Categoria { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla dTicketsTecnicos - Técnicos asignados a tickets
    /// </summary>
    [Table("dTicketsTecnicos")]
    public class dTicketsTecnicos
    {
        [Key, Column(Order = 0)]
        [Display(Name = "ID Ticket")]
        public int IdTicket { get; set; }

        [Key, Column(Order = 1)]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdTicket")]
        public virtual Ticket? Ticket { get; set; }

        [ForeignKey("IdPersona")]
        public virtual mEmpleados? Tecnico { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla mEmpresas - Empresas
    /// </summary>
    [Table("mEmpresas")]
    public class mEmpresas
    {
        [Key]
        [Display(Name = "ID Empresa")]
        public byte IdEmpresa { get; set; }

        [Display(Name = "Persona")]
        [Required]
        public int Persona { get; set; }

        [Display(Name = "Logo")]
        public byte[]? Logo { get; set; }

        // Propiedades de navegación
        [ForeignKey("Persona")]
        public virtual mPersonas? PersonaInfo { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla dEmpleados - Relación empleados-puestos
    /// </summary>
    [Table("dEmpleados")]
    public class dEmpleados
    {
        [Key, Column(Order = 0)]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        [Key, Column(Order = 1)]
        [Display(Name = "ID Puesto")]
        public byte IdPuesto { get; set; }

        [Display(Name = "Principal")]
        [Required]
        public bool Principal { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdPersona")]
        public virtual mEmpleados? Empleado { get; set; }
        
        [ForeignKey("IdPersona")]
        public virtual mPersonas? PersonaNavigation { get; set; }

        [ForeignKey("IdPuesto")]
        public virtual mPuestos? Puesto { get; set; }
    }

    /// <summary>
    /// Modelo para la tabla mPuestos - Puestos de trabajo
    /// </summary>
    [Table("mPuestos")]
    public class mPuestos
    {
        [Key]
        [Display(Name = "ID Puesto")]
        public byte IdPuesto { get; set; }

        [Display(Name = "Puesto")]
        [Required, StringLength(50)]
        public string Puesto { get; set; } = string.Empty;

        [Display(Name = "ID Departamento")]
        [Required]
        public byte IdDepto { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdDepto")]
        public virtual mDepartamentos? Departamento { get; set; }
    }

    /// <summary>
    /// ViewModel para el dashboard de tickets
    /// </summary>
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

    /// <summary>
    /// ViewModel para estadísticas de tickets
    /// </summary>
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
