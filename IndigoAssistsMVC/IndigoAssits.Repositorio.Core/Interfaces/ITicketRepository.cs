using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket>
    {
        Task<IEnumerable<TicketVista>> GetTicketsVistaAsync();
        Task<IEnumerable<TicketVista>> GetTicketsByUsuarioAsync(int usuarioId);
        Task<IEnumerable<TicketVista>> GetTicketsByTecnicoAsync(int tecnicoId);
        Task<IEnumerable<TicketVista>> GetTicketsByEstadoAsync(byte estado);
        Task<IEnumerable<TicketVista>> GetTicketsByPrioridadAsync(byte prioridad);
        Task<IEnumerable<TicketVista>> GetTicketsByCategoriaAsync(byte categoriaId);
        Task<IEnumerable<TicketVista>> GetTicketsByDepartamentoAsync(byte departamentoId);
        Task<IEnumerable<TicketVista>> GetTicketsByFechaAsync(DateTime fechaInicio, DateTime fechaFin);

        Task<IEnumerable<TicketVista>> GetTicketsWithFiltersAsync(
            int? usuarioId = null,
            int? tecnicoId = null,
            byte? estado = null,
            byte? prioridad = null,
            byte? categoriaId = null,
            byte? departamentoId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string? busquedaTexto = null);

        Task<(IEnumerable<TicketVista> Items, int TotalCount)> GetTicketsPagedAsync(
            int page,
            int pageSize,
            int? usuarioId = null,
            int? tecnicoId = null,
            byte? estado = null,
            byte? prioridad = null,
            byte? categoriaId = null,
            byte? departamentoId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null,
            string? busquedaTexto = null);

        Task<bool> AsignarTicketAsync(int ticketId, int tecnicoId, DateTime? fechaCompromiso = null);
        Task<bool> DesasignarTicketAsync(int ticketId);
        Task<bool> CambiarEstadoTicketAsync(int ticketId, byte nuevoEstado);


        Task<Dictionary<string, int>> GetEstadisticasPorEstadoAsync();
        Task<Dictionary<string, int>> GetEstadisticasPorPrioridadAsync();
        Task<Dictionary<string, int>> GetEstadisticasPorDepartamentoAsync();
        Task<Dictionary<string, int>> GetEstadisticasPorTecnicoAsync();
        Task<int> GetTotalTicketsAbiertosByDepartamentoAsync(byte departamentoId);
        Task<int> GetTotalTicketsEnProcesoByDepartamentoAsync(byte departamentoId);
        Task<int> GetTotalTicketsCerradosByDepartamentoAsync(byte departamentoId);

        Task<TicketDashboardViewModel> GetDashboardDataAsync(int? usuarioId = null);
        Task<IEnumerable<TicketVista>> GetTicketsRecientesAsync(int cantidad = 10);
        Task<IEnumerable<TicketVista>> GetTicketsVencidosAsync();
        Task<IEnumerable<TicketVista>> GetTicketsPorVencerAsync(int diasAntes = 3);

        Task<IEnumerable<TicketVista>> BuscarTicketsAsync(string terminoBusqueda);
    }
}
