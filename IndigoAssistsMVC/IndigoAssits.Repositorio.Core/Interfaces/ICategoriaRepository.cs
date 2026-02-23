using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{
    public interface ICategoriaRepository : IGenericRepository<mCategoriasTicket>
    {
        Task<IEnumerable<mCategoriasTicket>> GetCategoriasByDepartamentoAsync(byte departamentoId);
        Task<IEnumerable<mCategoriasTicket>> GetCategoriasAsync();
        Task<mCategoriasTicket?> GetCategoriaWithSubcategoriasAsync(byte categoriaId);
        Task<IEnumerable<mSubCategoriasTicket>> GetSubcategoriasByIsAsync(byte subcategoriaId);
        Task<IEnumerable<mSubCategoriasTicket>> GetSubcategoriasAsync();

        Task<IEnumerable<mSubCategoriasTicket>> GetSubcategoriasForDropdownAsync(byte categoriaId);
        Task<IEnumerable<mSubCategoriasTicket>> GetAllSubcategoriasForDropdownAsync();

        Task<Dictionary<string, int>> GetEstadisticasCategoriasAsync();
        Task<int> GetTotalTicketsPorCategoriaAsync(byte categoriaId);
        Task<int> GetTotalTicketsPorSubcategoriaAsync(byte subcategoriaId);
    }
}
