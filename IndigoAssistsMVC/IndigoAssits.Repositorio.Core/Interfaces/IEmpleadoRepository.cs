using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{
    public interface IEmpleadoRepository : IGenericRepository<mEmpleados>
    {
        Task<IEnumerable<mEmpleados>> GetEmpleadosTecnicosAsync();
        Task<IEnumerable<mEmpleados>> GetEmpleadosByDepartamentoAsync(byte departamentoId);
        Task<mEmpleados?> GetEmpleadoWithPersonaAsync(int idPersona);
        Task<mEmpleados?> GetEmpleadoByLoginAsync(string login);

        Task<IEnumerable<mEmpleados>> GetEmpleadosWithPersonaAsync();

        Task<Dictionary<string, int>> GetEstadisticasEmpleadosAsync();
        Task<int> GetTotalEmpleadosActivosAsync();
        Task<int> GetTotalEmpleadosByDepartamentoAsync(byte departamentoId);
    }
}
