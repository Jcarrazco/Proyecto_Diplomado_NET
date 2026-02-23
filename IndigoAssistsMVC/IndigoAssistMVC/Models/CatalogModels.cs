using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndigoAssistMVC.Models
{
    [Table("mTiposActivo")]
    public class TipoActivo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte IdTipoActivo { get; set; }

        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de Activo")]
        [Column("TipoActivo")]
        public string TipoActivoNombre { get; set; } = string.Empty;

        // Navegación
        public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
    }


    [Table("mStatus")]
    public class Status
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte StatusId { get; set; }

        [Required]
        [StringLength(20)]
        [DisplayName("Status")]
        [Column("Status")]
        public string StatusNombre { get; set; } = string.Empty;

        // Navegación
        public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
    }

    [Table("mProveedores")]
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte IdProveedor { get; set; }

        [Required]
        [StringLength(120)]
        [DisplayName("Proveedor")]
        [Column("Proveedor")]
        public string ProveedorNombre { get; set; } = string.Empty;

        // Navegación
        public virtual ICollection<Activo> Activos { get; set; } = new List<Activo>();
    }

    [Table("mComponentes")]
    public class Componente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte IdComponente { get; set; }

        [Required]
        [StringLength(80)]
        [DisplayName("Componente")]
        [Column("Componente")]
        public string ComponenteNombre { get; set; } = string.Empty;

        [DisplayName("Valor Bit")]
        public int? ValorBit { get; set; }
    }

    [Table("mSoftware")]
    public class Software
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte IdSoftware { get; set; }

        [Required]
        [StringLength(80)]
        [DisplayName("Software")]
        public string Nombre { get; set; } = string.Empty;
    }
}
