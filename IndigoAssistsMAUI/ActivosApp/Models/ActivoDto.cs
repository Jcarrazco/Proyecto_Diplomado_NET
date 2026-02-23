using System;

namespace ActivosApp.Models
{
    public class ActivoDto
    {
        public string Codigo { get; set; } = string.Empty;

        public string? Marca { get; set; }

        public string? Modelo { get; set; }

        public string? Serie { get; set; }

        public string? Nombre { get; set; }

        public string? PersonaAsign { get; set; }

        public string? Ubicacion { get; set; }

        public DateTimeOffset? FeCompra { get; set; }

        public DateTimeOffset? FeAlta { get; set; }

        public DateTimeOffset? FeBaja { get; set; }

        public double? CostoCompra { get; set; }

        public string? Notas { get; set; }

        public int? IdTipoActivo { get; set; }

        public string? TipoActivoNombre { get; set; }

        public int? IdDepartamento { get; set; }

        public string? DepartamentoNombre { get; set; }

        public int? IdStatus { get; set; }

        public string? StatusNombre { get; set; }

        public int? IdProveedor { get; set; }

        public string? ProveedorNombre { get; set; }

        public int? CodificacionComponentes { get; set; }

        public bool? TieneSoftwareOP { get; set; }
    }
}
