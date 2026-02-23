using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ActivosApp.Models
{
    public class ActivoCreateDto
    {
        [Required]
        [StringLength(40)]
        public string Codigo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Marca { get; set; }

        [StringLength(80)]
        public string? Modelo { get; set; }

        [StringLength(80)]
        public string? Serie { get; set; }

        [StringLength(120)]
        public string? Nombre { get; set; }

        [StringLength(120)]
        public string? PersonaAsign { get; set; }

        [StringLength(120)]
        public string? Ubicacion { get; set; }

        public DateTime? FeCompra { get; set; }

        public DateTime FeAlta { get; set; }

        public DateTime? FeBaja { get; set; }

        public double? CostoCompra { get; set; }

        [StringLength(400)]
        public string? Notas { get; set; }

        public int? IdTipoActivo { get; set; }

        [StringLength(50)]
        public string? TipoActivoNombre { get; set; }

        public int? IdDepartamento { get; set; }

        [StringLength(50)]
        public string? DepartamentoNombre { get; set; }

        public int? IdStatus { get; set; }

        [StringLength(20)]
        public string? StatusNombre { get; set; }

        public int? IdProveedor { get; set; }

        [StringLength(120)]
        public string? ProveedorNombre { get; set; }

        public int? CodificacionComponentes { get; set; }

        public bool? TieneSoftwareOP { get; set; }
    }
}
