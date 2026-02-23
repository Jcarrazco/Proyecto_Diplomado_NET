using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using IndigoAssistMVC.Models;
using IndigoAssistMVC.ViewModels;
using IndigoAssistMVC.Services;

namespace IndigoAssistMVC.Controllers
{
    [Authorize(Roles = "Administrador,Supervisor")]
    public class ActivoController : Controller
    {
        private readonly IActivoApiService _activoApiService;
        private readonly ICatalogoApiService _catalogoApiService;

        public ActivoController(
            IActivoApiService activoApiService,
            ICatalogoApiService catalogoApiService)
        {
            _activoApiService = activoApiService;
            _catalogoApiService = catalogoApiService;
        }

        // GET: Activo
        public async Task<IActionResult> Index(string mode, ActivoFiltroViewModel filtro)
        {
            var viewModel = new ActivoIndexViewModel();
            
            // Cargar datos para los dropdowns (desde DBContext - catálogos)
            await CargarDatosParaFiltros(viewModel.Filtro);

            if (mode == "filter")
            {
                // Aplicar filtros usando la API
                viewModel.Filtro = filtro;
                // Re-cargar listas del filtro con el modelo asignado para evitar nulos en SelectList
                await CargarDatosParaFiltros(viewModel.Filtro);
                var activos = await _activoApiService.GetActivosAsync(filtro);
                viewModel.Resultados = activos;
            }
            else
            {
                // Mostrar todos los activos usando la API
                try
                {
                    var activos = await _activoApiService.GetActivosAsync();
                    viewModel.Resultados = activos;
                    
                    // Log para diagnóstico
                    if (activos == null || activos.Count == 0)
                    {
                        // Verificar si hay un problema de autenticación
                        var token = HttpContext.Session.GetString("ApiToken");
                        if (string.IsNullOrEmpty(token))
                        {
                            TempData["Error"] = "No se pudo obtener el token de autenticación. Por favor, inicie sesión nuevamente.";
                        }
                        else
                        {
                            TempData["Info"] = "No se encontraron activos en la base de datos.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log del error
                    TempData["Error"] = $"Error al cargar activos: {ex.Message}";
                    viewModel.Resultados = new List<ActivoViewModel>();
                }
            }

            return View(viewModel);
        }

        // GET: Activo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activoViewModel = await _activoApiService.GetActivoPorIdAsync(id.Value);
            
            if (activoViewModel == null)
            {
                return NotFound();
            }

            // Cargar componentes disponibles para mostrar los seleccionados (desde API)
            activoViewModel.ComponentesDisponibles = await _catalogoApiService.GetComponentesAsync();

            return View(activoViewModel);
        }

        // GET: Activo/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = await PrepareViewModelAsync();
            return View(viewModel);
        }

        // POST: Activo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivoViewModel viewModel, int[] ComponentesSeleccionados)
        {
            // Procesar selección múltiple de componentes
            if (ComponentesSeleccionados != null && ComponentesSeleccionados.Length > 0)
            {
                viewModel.CodificacionComponentes = ComponentesSeleccionados.Sum();
            }
            else
            {
                viewModel.CodificacionComponentes = 0;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var id = await _activoApiService.CrearActivoAsync(viewModel);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al crear el activo: {ex.Message}");
                }
            }

            viewModel = await PrepareViewModelAsync(viewModel);
            return View(viewModel);
        }

        // GET: Activo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activoViewModel = await _activoApiService.GetActivoPorIdAsync(id.Value);
            
            if (activoViewModel == null)
            {
                return NotFound();
            }

            activoViewModel = await PrepareViewModelAsync(activoViewModel);

            return View(activoViewModel);
        }

        // POST: Activo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActivoViewModel viewModel, int[] ComponentesSeleccionados)
        {
            if (id != viewModel.IdActivo)
            {
                return NotFound();
            }

            if (ComponentesSeleccionados != null && ComponentesSeleccionados.Length > 0)
            {
                viewModel.CodificacionComponentes = ComponentesSeleccionados.Sum();
            }
            else
            {
                viewModel.CodificacionComponentes = 0;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var resultado = await _activoApiService.ActualizarActivoAsync(id, viewModel);
                    if (!resultado)
                    {
                        return NotFound();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error al actualizar el activo: {ex.Message}");
                }
            }

            viewModel = await PrepareViewModelAsync(viewModel);
            return View(viewModel);
        }

        // GET: Activo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activoViewModel = await _activoApiService.GetActivoPorIdAsync(id.Value);
            
            if (activoViewModel == null)
            {
                return NotFound();
            }

            // Cargar componentes disponibles para mostrar los seleccionados (desde API)
            activoViewModel.ComponentesDisponibles = await _catalogoApiService.GetComponentesAsync();

            return View(activoViewModel);
        }

        // POST: Activo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var resultado = await _activoApiService.EliminarActivoAsync(id);
                if (!resultado)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al eliminar el activo: {ex.Message}");
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ActivoExists(int id)
        {
            return await _activoApiService.ExisteActivoAsync(id);
        }

        /// <summary>
        /// Prepara el ViewModel con los datos de los catálogos (desde API)
        /// </summary>
        private async Task<ActivoViewModel> PrepareViewModelAsync(ActivoViewModel? viewModel = null)
        {
            viewModel ??= new ActivoViewModel();

            // Cargar catálogos desde la API
            var tiposActivo = await _catalogoApiService.GetTiposActivoAsync();
            var departamentos = await _catalogoApiService.GetDepartamentosAsync();
            var status = await _catalogoApiService.GetStatusActivoAsync();
            var proveedores = await _catalogoApiService.GetProveedoresAsync();
            var componentes = await _catalogoApiService.GetComponentesAsync();

            viewModel.TipoActivoList = new SelectList(tiposActivo, "IdTipoActivo", "TipoActivoNombre", viewModel.IdTipoActivo);
            viewModel.DepartamentoList = new SelectList(departamentos, "IdDepto", "Departamento", viewModel.IdDepartamento);
            viewModel.StatusList = new SelectList(status, "StatusId", "StatusNombre", viewModel.IdStatus);
            viewModel.ProveedorList = new SelectList(proveedores, "IdProveedor", "ProveedorNombre", viewModel.IdProveedor);

            // Preparar opciones de componentes
            viewModel.ComponentesOptions = componentes.Select(c => new SelectListItem
            {
                Text = c.ComponenteNombre,
                Value = c.ValorBit?.ToString() ?? "0",
                Selected = viewModel.CodificacionComponentes.HasValue && 
                          c.ValorBit.HasValue && 
                          (viewModel.CodificacionComponentes.Value & c.ValorBit.Value) == c.ValorBit.Value
            }).ToList();

            viewModel.ComponentesDisponibles = componentes;

            return viewModel;
        }

        /// <summary>
        /// Carga los datos de los catálogos para los filtros (desde API)
        /// </summary>
        private async Task CargarDatosParaFiltros(ActivoFiltroViewModel filtro)
        {
            // Cargar catálogos desde la API
            var tiposActivo = await _catalogoApiService.GetTiposActivoAsync();
            var departamentos = await _catalogoApiService.GetDepartamentosAsync();
            var status = await _catalogoApiService.GetStatusActivoAsync();
            var proveedores = await _catalogoApiService.GetProveedoresAsync();
            var componentes = await _catalogoApiService.GetComponentesAsync();

            filtro.TiposActivo = new SelectList(tiposActivo, "IdTipoActivo", "TipoActivoNombre", filtro.TipoActivoId);
            filtro.Departamentos = new SelectList(departamentos, "IdDepto", "Departamento", filtro.DepartamentoId);
            filtro.Statuses = new SelectList(status, "StatusId", "StatusNombre", filtro.StatusId);
            filtro.Proveedores = new SelectList(proveedores, "IdProveedor", "ProveedorNombre", filtro.ProveedorId);
            
            // Filtrar solo componentes con ValorBit válido y crear SelectListItems manualmente
            var componentesConValor = componentes.Where(c => c.ValorBit.HasValue).ToList();
            var componentesItems = componentesConValor.Select(c => new SelectListItem
            {
                Text = c.ComponenteNombre,
                Value = c.ValorBit!.Value.ToString()
            }).ToList();
            
            filtro.Componentes = new SelectList(componentesItems, "Value", "Text");
        }
    }
}
