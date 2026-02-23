using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using IndigoAssistMVC.Models;

namespace IndigoAssistMVC.ViewModels
{
    public class ActivoViewModel
    {
        // Propiedades del Activo
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

        [DisplayName("Fecha de Compra")]
        [DataType(DataType.Date)]
        public DateTime? FeCompra { get; set; }

        [DisplayName("Fecha de Alta")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime FeAlta { get; set; } = DateTime.Today;

        [DisplayName("Fecha de Baja")]
        [DataType(DataType.Date)]
        public DateTime? FeBaja { get; set; }

        [DisplayName("Costo de Compra")]
        [Range(typeof(decimal), "0", "10000000")]
        [DataType(DataType.Currency)]
        public decimal? CostoCompra { get; set; }

        [DisplayName("Notas")]
        [DataType(DataType.MultilineText)]
        [StringLength(400)]
        public string? Notas { get; set; }

        [DisplayName("Codificación de Componentes")]
        public int? CodificacionComponentes { get; set; }

        [DisplayName("Tiene Sistema Operativo")]
        public bool TieneSoftwareOP { get; set; }

        // IDs de las entidades relacionadas
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

        // Propiedades para mostrar los nombres en las vistas
        [DisplayName("Tipo de Activo")]
        public string? TipoActivoNombre { get; set; }

        [DisplayName("Departamento")]
        public string? DepartamentoNombre { get; set; }

        [DisplayName("Status")]
        public string? StatusNombre { get; set; }

        [DisplayName("Proveedor")]
        public string? ProveedorNombre { get; set; }

        // Listas para los dropdowns
        public SelectList? TipoActivoList { get; set; }
        public SelectList? DepartamentoList { get; set; }
        public SelectList? StatusList { get; set; }
        public SelectList? ProveedorList { get; set; }

        // Para el manejo de componentes
        public List<SelectListItem>? ComponentesOptions { get; set; }
        public int[]? ComponentesSeleccionados { get; set; }

        // Lista de componentes disponibles para mostrar información
        public List<Componente>? ComponentesDisponibles { get; set; }

        // Método para convertir de Activo a ViewModel
        public static ActivoViewModel FromActivo(Activo activo)
        {
            return new ActivoViewModel
            {
                IdActivo = activo.IdActivo,
                Codigo = activo.Codigo,
                Marca = activo.Marca,
                Modelo = activo.Modelo,
                Serie = activo.Serie,
                Nombre = activo.Nombre,
                PersonaAsign = activo.PersonaAsign,
                Ubicacion = activo.Ubicacion,
                FeCompra = activo.FeCompra,
                FeAlta = activo.FeAlta,
                FeBaja = activo.FeBaja,
                CostoCompra = activo.CostoCompra,
                Notas = activo.Notas,
                CodificacionComponentes = (int?)activo.CodificacionComponentes,
                TieneSoftwareOP = activo.TieneSoftwareOP,
                IdTipoActivo = activo.IdTipoActivo,
                IdDepartamento = activo.IdDepartamento,
                IdStatus = activo.IdStatus,
                IdProveedor = activo.IdProveedor,
                TipoActivoNombre = activo.TipoActivo?.TipoActivoNombre,
                DepartamentoNombre = activo.Departamento?.Departamento,
                StatusNombre = activo.Status.StatusNombre,
                ProveedorNombre = activo.Proveedor?.ProveedorNombre
            };
        }

        // Método para convertir de ViewModel a Activo
        public Activo ToActivo()
        {
            return new Activo
            {
                IdActivo = this.IdActivo,
                Codigo = this.Codigo,
                Marca = this.Marca,
                Modelo = this.Modelo,
                Serie = this.Serie,
                Nombre = this.Nombre,
                PersonaAsign = this.PersonaAsign,
                Ubicacion = this.Ubicacion,
                FeCompra = this.FeCompra,
                FeAlta = this.FeAlta,
                FeBaja = this.FeBaja,
                CostoCompra = this.CostoCompra,
                Notas = this.Notas,
                CodificacionComponentes = (CodificacionComponentes)(this.CodificacionComponentes ?? 0),
                TieneSoftwareOP = this.TieneSoftwareOP,
                IdTipoActivo = this.IdTipoActivo,
                IdDepartamento = this.IdDepartamento,
                IdStatus = this.IdStatus,
                IdProveedor = this.IdProveedor
            };
        }

        // Método para obtener los componentes seleccionados basado en la codificación
        public List<string> GetComponentesSeleccionadosNames(List<Componente> componentesDisponibles)
        {
            var componentesSeleccionados = new List<string>();
            
            if (CodificacionComponentes.HasValue && componentesDisponibles != null)
            {
                foreach (var componente in componentesDisponibles)
                {
                    if (componente.ValorBit.HasValue && 
                        (CodificacionComponentes.Value & componente.ValorBit.Value) == componente.ValorBit.Value)
                    {
                        componentesSeleccionados.Add(componente.ComponenteNombre);
                    }
                }
            }
            
            return componentesSeleccionados;
        }
    }
}
