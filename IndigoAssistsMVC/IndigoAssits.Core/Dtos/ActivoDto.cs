using System.ComponentModel.DataAnnotations;

namespace IndigoAssits.Core.Dtos
{
    public class ActivoDto
    {
        public int IdActivo { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Serie { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? PersonaAsign { get; set; }
        public string? Ubicacion { get; set; }
        public DateTime FeAlta { get; set; }
        public DateTime? FeCompra { get; set; }
        public DateTime? FeBaja { get; set; }
        public decimal? CostoCompra { get; set; }
        public string? Notas { get; set; }
        public int CodificacionComponentes { get; set; }
        public bool TieneSoftwareOP { get; set; }
        public byte? IdTipoActivo { get; set; }
        public byte? IdDepartamento { get; set; }
        public byte? IdStatus { get; set; }
        public byte? IdProveedor { get; set; }

        public string? TipoActivoNombre { get; set; }
        public string? DepartamentoNombre { get; set; }
        public string? StatusNombre { get; set; }
        public string? ProveedorNombre { get; set; }
    }

    public class ActivoCreateDto
    {
        [Required, StringLength(40)]
        public string Codigo { get; set; } = string.Empty;
        [StringLength(50)]
        public string? Marca { get; set; }
        [StringLength(80)]
        public string? Modelo { get; set; }
        [StringLength(80)]
        public string? Serie { get; set; }
        [Required, StringLength(120)]
        public string Nombre { get; set; } = string.Empty;
        [StringLength(120)]
        public string? PersonaAsign { get; set; }
        [StringLength(120)]
        public string? Ubicacion { get; set; }
        [DataType(DataType.Date)]
        public DateTime? FeCompra { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime FeAlta { get; set; } = DateTime.Today;
        [DataType(DataType.Date)]
        public DateTime? FeBaja { get; set; }
        public decimal? CostoCompra { get; set; }
        [StringLength(400)]
        public string? Notas { get; set; }
        public int CodificacionComponentes { get; set; }
        public bool TieneSoftwareOP { get; set; }
        [Required]
        public byte? IdTipoActivo { get; set; }
        [Required]
        public byte? IdDepartamento { get; set; }
        [Required]
        public byte? IdStatus { get; set; }
        [Required]
        public byte? IdProveedor { get; set; }
    }

    public class ActivoUpdateDto : ActivoCreateDto
    {
        [Required]
        public int IdActivo { get; set; }
    }

    public class ActivoFiltroDto
    {
        public int? IdActivo { get; set; }
        public string? CodigoLike { get; set; }
        public string? MarcaLike { get; set; }
        public string? NombreLike { get; set; }
        public string? PersonaAsignLike { get; set; }
        public string? UbicacionLike { get; set; }
        public byte? TipoActivoId { get; set; }
        public byte? DepartamentoId { get; set; }
        public byte? StatusId { get; set; }
        public byte? ProveedorId { get; set; }
        public bool? TieneSoftwareOP { get; set; }
        public decimal? CostoMin { get; set; }
        public decimal? CostoMax { get; set; }
        public DateTime? FechaAltaDesde { get; set; }
        public DateTime? FechaAltaHasta { get; set; }
        public DateTime? FechaCompraDesde { get; set; }
        public DateTime? FechaCompraHasta { get; set; }
        public DateTime? FechaBajaDesde { get; set; }
        public DateTime? FechaBajaHasta { get; set; }
        public List<int> ComponentesSeleccionados { get; set; } = new List<int>();
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 10;
    }

    public class ActivoPaginadoDto
    {
        public List<ActivoDto> Items { get; set; } = new List<ActivoDto>();
        public int TotalRegistros { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanoPagina { get; set; }
    }
}


