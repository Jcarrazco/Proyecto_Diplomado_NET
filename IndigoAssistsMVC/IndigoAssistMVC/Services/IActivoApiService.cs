using IndigoAssistMVC.ViewModels;

namespace IndigoAssistMVC.Services
{
    /// <summary>
    /// Interfaz para el servicio de consumo de la API de Activos
    /// </summary>
    public interface IActivoApiService
    {
        /// <summary>
        /// Obtiene todos los activos con filtros opcionales
        /// </summary>
        Task<List<ActivoViewModel>> GetActivosAsync(ActivoFiltroViewModel? filtro = null);

        /// <summary>
        /// Obtiene un activo por su ID
        /// </summary>
        Task<ActivoViewModel?> GetActivoPorIdAsync(int id);

        /// <summary>
        /// Crea un nuevo activo
        /// </summary>
        Task<int> CrearActivoAsync(ActivoViewModel viewModel);

        /// <summary>
        /// Actualiza un activo existente
        /// </summary>
        Task<bool> ActualizarActivoAsync(int id, ActivoViewModel viewModel);

        /// <summary>
        /// Elimina un activo por su ID
        /// </summary>
        Task<bool> EliminarActivoAsync(int id);

        /// <summary>
        /// Verifica si existe un activo con el ID especificado
        /// </summary>
        Task<bool> ExisteActivoAsync(int id);
    }
}

