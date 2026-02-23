using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using IndigoAssistMVC.Models;

namespace IndigoAssistMVC.ViewModels
{
    public class ActivoFiltroViewModel
    {
        [DisplayName("ID del Activo")]
        public int? IdActivo { get; set; }

        [DisplayName("Código")]
        public string? CodigoLike { get; set; }

        [DisplayName("Marca")]
        public string? MarcaLike { get; set; }

        [DisplayName("Nombre")]
        public string? NombreLike { get; set; }

        [DisplayName("Persona Asignada")]
        public string? PersonaAsignLike { get; set; }

        [DisplayName("Ubicación")]
        public string? UbicacionLike { get; set; }

        [DisplayName("Tipo de Activo")]
        public byte? TipoActivoId { get; set; }

        [DisplayName("Departamento")]
        public byte? DepartamentoId { get; set; }

        [DisplayName("Status")]
        public byte? StatusId { get; set; }

        [DisplayName("Proveedor")]
        public byte? ProveedorId { get; set; }

        [DisplayName("Tiene Sistema Operativo")]
        public bool? TieneSoftwareOP { get; set; }

        [DisplayName("Costo Mínimo")]
        public decimal? CostoMin { get; set; }

        [DisplayName("Costo Máximo")]
        public decimal? CostoMax { get; set; }

        [DisplayName("Fecha de Alta Desde")]
        public DateTime? FechaAltaDesde { get; set; }

        [DisplayName("Fecha de Alta Hasta")]
        public DateTime? FechaAltaHasta { get; set; }

        [DisplayName("Fecha de Compra Desde")]
        public DateTime? FechaCompraDesde { get; set; }

        [DisplayName("Fecha de Compra Hasta")]
        public DateTime? FechaCompraHasta { get; set; }

        [DisplayName("Fecha de Baja Desde")]
        public DateTime? FechaBajaDesde { get; set; }

        [DisplayName("Fecha de Baja Hasta")]
        public DateTime? FechaBajaHasta { get; set; }

        [DisplayName("Componentes")]
        public List<int> ComponentesSeleccionados { get; set; } = new List<int>();

        // Listas para los dropdowns
        public SelectList TiposActivo { get; set; } = new SelectList(new List<object>());
        public SelectList Departamentos { get; set; } = new SelectList(new List<object>());
        public SelectList Statuses { get; set; } = new SelectList(new List<object>());
        public SelectList Proveedores { get; set; } = new SelectList(new List<object>());
        public SelectList Componentes { get; set; } = new SelectList(new List<object>());

        // Enum para tipo de fecha a filtrar
        public enum DateTarget
        {
            None,
            FechaAlta,
            FechaCompra,
            FechaBaja
        }

        [DisplayName("Tipo de Fecha")]
        public DateTarget FechaTarget { get; set; } = DateTarget.None;
    }
}
