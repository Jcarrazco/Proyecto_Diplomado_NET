using System;
using System.Collections.Generic;
using System.Text;

namespace ActivosApp.Models
{
    public class ActivoResumenDto
    {
        public int IdActivo { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string? Marca { get; set; }

        public string? Modelo { get; set; }

        public string? Serie { get; set; }

        public string? Nombre { get; set; }

        public string? PersonaAsign { get; set; }

        public string? Ubicacion { get; set; }

        public DateTime? FeCompra { get; set; }

        public DateTime FeAlta { get; set; }

        public DateTime? FeBaja { get; set; }

        public double? CostoCompra { get; set; }

        public string? Notas { get; set; }

        public int? CodificacionComponentes { get; set; }

        public bool? TieneSoftwareOP { get; set; }

        public string? TipoActivoNombre { get; set; }

        public string? DepartamentoNombre { get; set; }

        public string? StatusNombre { get; set; }

        public string? ProveedorNombre { get; set; }
    }
}
