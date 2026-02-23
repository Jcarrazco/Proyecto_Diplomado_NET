using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigoAssistMVC.Models
{
    // Mantenemos solo el enum de CodificacionComponentes para el manejo de bits
    [Flags]
    public enum CodificacionComponentes
    {
        Ninguno = 0,
        Procesador = 1 << 0,        // = 1
        MemoriaRAM = 1 << 1,        // = 2
        DiscoDuro = 1 << 2,         // = 4
        TarjetaGrafica = 1 << 3,    // = 8
        TarjetaRed = 1 << 4,        // = 16
        Fuente = 1 << 5,            // = 32
        Ventiladores = 1 << 6,      // = 64
        Pantalla = 1 << 7,          // = 128
        Teclado = 1 << 8,           // = 256
        Mouse = 1 << 9,             // = 512
        Bocinas = 1 << 10,          // = 1024
        Camara = 1 << 11            // = 2048
    }

    [Table("mActivos")]
    public class Activo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("ID")]
        public int IdActivo { get; set; }

        [DisplayName("Código")]
        [Required, StringLength(40)]
        public string Codigo { get; set; } = string.Empty;

        [DisplayName("Marca")]
        [StringLength(50)]
        public string? Marca { get; set; }

        [DisplayName("Modelo")]
        [StringLength(80)]
        public string? Modelo { get; set; }

        [DisplayName("Serie")]
        [StringLength(80)]
        public string? Serie { get; set; }

        [DisplayName("Nombre")]
        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;

        [DisplayName("Persona Asignada")]
        [StringLength(120)]
        public string? PersonaAsign { get; set; }

        [DisplayName("Ubicación")]
        [StringLength(120)]
        public string? Ubicacion { get; set; }

        [DisplayName("Fecha de Alta")]
        [Required]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime FeAlta { get; set; } = DateTime.Today;

        [DisplayName("Fecha de Compra")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? FeCompra { get; set; }

        [DisplayName("Fecha de Baja")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime? FeBaja { get; set; }

        [DisplayName("Costo de Compra")]
        [Column(TypeName = "decimal(12, 2)")]
        [Range(typeof(decimal), "0", "10000000")]
        [DataType(DataType.Currency)]
        public decimal? CostoCompra { get; set; }

        [DisplayName("Notas")]
        [DataType(DataType.MultilineText)]
        [StringLength(400)]
        public string? Notas { get; set; }

        [DisplayName("Codificación de Componentes")]
        public CodificacionComponentes CodificacionComponentes { get; set; } = CodificacionComponentes.Ninguno;

        [DisplayName("Tiene Sistema Operativo")]
        public bool TieneSoftwareOP { get; set; } //Switch

        [DisplayName("Tipo de Activo")]
        [Required]
        public byte? IdTipoActivo { get; set; }

        [DisplayName("Departamento")]
        [Required]
        public byte? IdDepartamento { get; set; }

        [DisplayName("Status")]
        [Required]
        public byte? IdStatus { get; set; }

        [DisplayName("Proveedor")]
        [Required]
        public byte? IdProveedor { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdTipoActivo")]
        public virtual TipoActivo? TipoActivo { get; set; }

        [ForeignKey("IdDepartamento")]
        public virtual mDepartamentos? Departamento { get; set; }

        [ForeignKey("IdStatus")]
        public virtual Status? Status { get; set; }

        [ForeignKey("IdProveedor")]
        public virtual Proveedor? Proveedor { get; set; }
    }
}
