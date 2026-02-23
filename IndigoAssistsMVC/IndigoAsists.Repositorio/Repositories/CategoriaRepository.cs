using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAsists.Repositorio.Db;

namespace IndigoAsists.Repositorio.Repositories
{
    public class CategoriaRepository : GenericRepository<mCategoriasTicket>, ICategoriaRepository
    {
        private readonly IndigoLegacyDbContext _legacyContext;

        public CategoriaRepository(IndigoLegacyDbContext context) : base(context)
        {
            _legacyContext = context;
        }

        public async Task<IEnumerable<mCategoriasTicket>> GetCategoriasByDepartamentoAsync(byte departamentoId)
        {
            return await _dbSet
                .Where(c => c.IdDepto == departamentoId)
                .Include(c => c.Departamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<mCategoriasTicket>> GetCategoriasAsync()
        {
            return await _dbSet
                .Include(c => c.Departamento)
                .ToListAsync();
        }

        public async Task<mCategoriasTicket?> GetCategoriaWithSubcategoriasAsync(byte categoriaId)
        {
            return await _dbSet
                .Where(c => c.IdCategoria == categoriaId)
                .Include(c => c.Departamento)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<mSubCategoriasTicket>> GetSubcategoriasByIsAsync(byte subcategoriaId)
        {
            return await _legacyContext.SubCategoriasTickets
                .Where(sc => sc.IdSubCategoria == subcategoriaId)
                .Include(sc => sc.Categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<mSubCategoriasTicket>> GetSubcategoriasAsync()
        {
            return await _legacyContext.SubCategoriasTickets
                .Include(sc => sc.Categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<mSubCategoriasTicket>> GetSubcategoriasForDropdownAsync(byte categoriaId)
        {
            return await _legacyContext.SubCategoriasTickets
                .Where(sc => sc.IdCategoria == categoriaId)
                .OrderBy(sc => sc.SubCategoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<mSubCategoriasTicket>> GetAllSubcategoriasForDropdownAsync()
        {
            return await _legacyContext.SubCategoriasTickets
                .OrderBy(sc => sc.SubCategoria)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetEstadisticasCategoriasAsync()
        {
            var estadisticas = new Dictionary<string, int>();

            estadisticas["TotalCategorias"] = await _dbSet.CountAsync();

            estadisticas["TotalSubcategorias"] = await _legacyContext.SubCategoriasTickets.CountAsync();

            var categoriasPorDepto = await _dbSet
                .GroupBy(c => c.IdDepto)
                .ToDictionaryAsync(g => $"Depto_{g.Key}", g => g.Count());

            foreach (var item in categoriasPorDepto)
            {
                estadisticas[item.Key] = item.Value;
            }

            return estadisticas;
        }

        public async Task<int> GetTotalTicketsPorCategoriaAsync(byte categoriaId)
        {
            return await _legacyContext.TicketVistas
                .CountAsync(t => t.IdCategoria == categoriaId);
        }

        public async Task<int> GetTotalTicketsPorSubcategoriaAsync(byte subcategoriaId)
        {
            return await _legacyContext.TicketVistas
                .CountAsync(t => t.IdSubCategoria == subcategoriaId);
        }
    }
}
