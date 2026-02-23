using IndigoAssistMVC.Models;

namespace IndigoAssistMVC.Services
{
    /// <summary>
    /// Interfaz para el servicio de consumo de la API de Catálogos
    /// </summary>
    public interface ICatalogoApiService
    {
        /// <summary>
        /// Obtiene todos los tipos de activo
        /// </summary>
        Task<List<TipoActivo>> GetTiposActivoAsync();

        /// <summary>
        /// Obtiene todos los status de activos
        /// </summary>
        Task<List<Status>> GetStatusActivoAsync();

        /// <summary>
        /// Obtiene todos los proveedores
        /// </summary>
        Task<List<Proveedor>> GetProveedoresAsync();

        /// <summary>
        /// Obtiene todos los componentes
        /// </summary>
        Task<List<Componente>> GetComponentesAsync();

        /// <summary>
        /// Obtiene todos los departamentos
        /// </summary>
        Task<List<mDepartamentos>> GetDepartamentosAsync();
    }
}

