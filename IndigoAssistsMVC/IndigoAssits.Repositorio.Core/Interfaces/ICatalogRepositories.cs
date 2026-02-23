using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{
    public interface IEstadoTicketRepository : IGenericRepository<mStatusTicket>
    {
        Task<IEnumerable<mStatusTicket>> GetEstadosActivosAsync();
        Task<mStatusTicket?> GetEstadoByCodigoAsync(byte status);
    }

    public interface IPrioridadTicketRepository : IGenericRepository<mPrioridadTicket>
    {
        Task<IEnumerable<mPrioridadTicket>> GetPrioridadesActivasAsync();
        Task<mPrioridadTicket?> GetPrioridadByIdAsync(byte idPrioridad);
    }

    public interface ITipoTicketRepository : IGenericRepository<mTipoTicket>
    {
        Task<IEnumerable<mTipoTicket>> GetTiposForDropdownAsync();
        Task<mTipoTicket?> GetTipoByIdAsync(byte idTipoTicket);
    }

    public interface IPersonaRepository : IGenericRepository<mPersonas>
    {
        Task<IEnumerable<mPersonas>> GetPersonasActivasAsync();
        Task<mPersonas?> GetPersonaByEmailAsync(string email);
        Task<IEnumerable<mPersonas>> BuscarPersonasAsync(string terminoBusqueda);
        Task<bool> ExisteEmailAsync(string email);
    }


    public interface IPuestoRepository : IGenericRepository<mPuestos>
    {
        Task<IEnumerable<mPuestos>> GetPuestosByDepartamentoAsync(byte departamentoId);
        Task<IEnumerable<mPuestos>> GetPuestosActivosAsync();
        Task<IEnumerable<mPuestos>> GetPuestosForDropdownAsync();
        Task<bool> ExistePuestoAsync(string nombrePuesto, byte departamentoId);
    }
}
