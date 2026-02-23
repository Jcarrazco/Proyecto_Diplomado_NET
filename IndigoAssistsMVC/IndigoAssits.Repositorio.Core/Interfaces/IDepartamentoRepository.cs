using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{
    /// <summary>
    /// Interfaz específica para operaciones de departamentos
    /// </summary>
    public interface IDepartamentoRepository : IGenericRepository<mDepartamentos>
    {
        // Consultas específicas de departamentos
        Task<IEnumerable<mDepartamentos>> GetDepartamentosConTicketsAsync();
        Task<IEnumerable<mDepartamentos>> GetDepartamentosActivosAsync();
        Task<mDepartamentos?> GetDepartamentoWithPuestosAsync(byte departamentoId);

        // Consultas para dropdowns
        Task<IEnumerable<mDepartamentos>> GetDepartamentosForDropdownAsync();
        Task<IEnumerable<mDepartamentos>> GetDepartamentosConTicketsForDropdownAsync();

        // Validaciones
        Task<bool> ExisteDepartamentoAsync(string nombreDepartamento);
        Task<bool> TieneEmpleadosAsync(byte departamentoId);
        Task<bool> TieneTicketsAsync(byte departamentoId);

        // Estadísticas
        Task<Dictionary<string, int>> GetEstadisticasDepartamentosAsync();
        Task<int> GetTotalEmpleadosPorDepartamentoAsync(byte departamentoId);
        Task<int> GetTotalTicketsPorDepartamentoAsync(byte departamentoId);
    }
}
