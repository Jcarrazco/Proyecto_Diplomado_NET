using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigoAssits.Repositorio.Core.Entities
{
    [Table("mPerEmp")]
    public class mPerEmp
    {
        [Key]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        [Display(Name = "Persona")]
        [Required]
        public int Persona { get; set; }

        [ForeignKey("Persona")]
        public virtual mPersonas? PersonaInfo { get; set; }

    }

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

    [Table("dTicketsTecnicos")]
    public class dTicketsTecnicos
    {
        [Key]
        [Display(Name = "ID Ticket")]
        public int IdTicket { get; set; }

        [Key]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdTicket")]
        public virtual Ticket? Ticket { get; set; }

        [ForeignKey("IdPersona")]
        public virtual mEmpleados? Tecnico { get; set; }
    }

    [Table("dEmpleados")]
    public class dEmpleados
    {
        [Key]
        [Display(Name = "ID Persona")]
        public int IdPersona { get; set; }

        [Key]
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

    [Table("mActivos")]
    public class Activo
    {
        [Key]
        [Display(Name = "ID Activo")]
        public int IdActivo { get; set; }

        [Display(Name = "Código")]
        [Required, StringLength(40)]
        public string Codigo { get; set; } = string.Empty;

        [Display(Name = "Marca")]
        [StringLength(50)]
        public string? Marca { get; set; }

        [Display(Name = "Modelo")]
        [StringLength(80)]
        public string? Modelo { get; set; }

        [Display(Name = "Serie")]
        [StringLength(80)]
        public string? Serie { get; set; }

        [Display(Name = "Nombre")]
        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Persona Asignada")]
        [StringLength(120)]
        public string? PersonaAsign { get; set; }

        [Display(Name = "Ubicación")]
        [StringLength(120)]
        public string? Ubicacion { get; set; }

        [Display(Name = "Fecha de Alta")]
        [Required]
        [Column(TypeName = "date")]
        public DateTime FeAlta { get; set; } = DateTime.Today;

        [Display(Name = "Fecha de Compra")]
        [Column(TypeName = "date")]
        public DateTime? FeCompra { get; set; }

        [Display(Name = "Fecha de Baja")]
        [Column(TypeName = "date")]
        public DateTime? FeBaja { get; set; }

        [Display(Name = "Costo de Compra")]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal? CostoCompra { get; set; }

        [Display(Name = "Notas")]
        [StringLength(400)]
        public string? Notas { get; set; }

        [Display(Name = "Codificación de Componentes")]
        public int? CodificacionComponentes { get; set; }

        [Display(Name = "Tiene Sistema Operativo")]
        public bool? TieneSoftwareOP { get; set; }

        [Display(Name = "Tipo de Activo")]
        public byte? IdTipoActivo { get; set; }

        [Display(Name = "Departamento")]
        public byte? IdDepartamento { get; set; }

        [Display(Name = "Status")]
        public byte? IdStatus { get; set; }

        [Display(Name = "Proveedor")]
        public byte? IdProveedor { get; set; }

        [ForeignKey("IdTipoActivo")]
        public virtual mTipoActivo? TipoActivo { get; set; }

        [ForeignKey("IdDepartamento")]
        public virtual mDepartamentos? Departamento { get; set; }

        [ForeignKey("IdStatus")]
        public virtual mStatus? Status { get; set; }

        [ForeignKey("IdProveedor")]
        public virtual mProveedor? Proveedor { get; set; }
    }

    [Table("mTiposActivo")]
    public class mTipoActivo
    {
        [Key]
        [Display(Name = "ID Tipo Activo")]
        public byte IdTipoActivo { get; set; }

        [Display(Name = "Tipo Activo")]
        [Required, StringLength(50)]
        [Column("TipoActivo")]
        public string TipoActivoNombre { get; set; } = string.Empty;

        public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
    }

    [Table("mStatus")]
    public class mStatus
    {
        [Key]
        [Display(Name = "ID Status")]
        public byte StatusId { get; set; }

        [Display(Name = "Status")]
        [Required, StringLength(20)]
        [Column("Status")]
        public string StatusNombre { get; set; } = string.Empty;

        public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
    }

    [Table("mProveedores")]
    public class mProveedor
    {
        [Key]
        [Display(Name = "ID Proveedor")]
        public byte IdProveedor { get; set; }

        [Display(Name = "Proveedor")]
        [Required, StringLength(120)]
        [Column("Proveedor")]
        public string ProveedorNombre { get; set; } = string.Empty;

        public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
    }

    [Table("mComponentes")]
    public class mComponente
    {
        [Key]
        [Display(Name = "ID Componente")]
        public byte IdComponente { get; set; }

        [Display(Name = "Componente")]
        [Required, StringLength(80)]
        [Column("Componente")]
        public string ComponenteNombre { get; set; } = string.Empty;

        [Display(Name = "Valor Bit")]
        public int? ValorBit { get; set; }
    }
}
