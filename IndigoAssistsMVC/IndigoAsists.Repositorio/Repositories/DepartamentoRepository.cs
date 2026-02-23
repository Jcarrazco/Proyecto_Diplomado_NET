using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAsists.Repositorio.Db;

namespace IndigoAsists.Repositorio.Repositories
{
    public class DepartamentoRepository : GenericRepository<mDepartamentos>, IDepartamentoRepository
    {
        private readonly IndigoLegacyDbContext _legacyContext;

        public DepartamentoRepository(IndigoLegacyDbContext context) : base(context)
        {
            _legacyContext = context;
        }

        public async Task<IEnumerable<mDepartamentos>> GetDepartamentosConTicketsAsync()
        {
            return await _dbSet
                .Where(d => d.Tickets)
                .ToListAsync();
        }

        public async Task<IEnumerable<mDepartamentos>> GetDepartamentosActivosAsync()
        {
            return await _dbSet.ToListAsync(); 
        }

        public async Task<mDepartamentos?> GetDepartamentoWithPuestosAsync(byte departamentoId)
        {
            return await _dbSet
                .Where(d => d.IdDepto == departamentoId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<mDepartamentos>> GetDepartamentosForDropdownAsync()
        {
            return await _dbSet
                .OrderBy(d => d.Departamento)
                .ToListAsync();
        }

        public async Task<IEnumerable<mDepartamentos>> GetDepartamentosConTicketsForDropdownAsync()
        {
            return await _dbSet
                .Where(d => d.Tickets)
                .OrderBy(d => d.Departamento)
                .ToListAsync();
        }

        public async Task<bool> ExisteDepartamentoAsync(string nombreDepartamento)
        {
            return await _dbSet.AnyAsync(d => d.Departamento == nombreDepartamento);
        }

        public async Task<bool> TieneEmpleadosAsync(byte departamentoId)
        {
            return await _legacyContext.Empleados
                .AnyAsync(e => e.IdPersona != 0); 
        }

        public async Task<bool> TieneTicketsAsync(byte departamentoId)
        {
            return await _legacyContext.TicketVistas
                .AnyAsync(t => t.IdDepto == departamentoId);
        }

        public async Task<Dictionary<string, int>> GetEstadisticasDepartamentosAsync()
        {
            var estadisticas = new Dictionary<string, int>();

            estadisticas["TotalDepartamentos"] = await _dbSet.CountAsync();

            estadisticas["DepartamentosConTickets"] = await _dbSet.CountAsync(d => d.Tickets);

            var ticketsPorDepto = await _legacyContext.TicketVistas
                .GroupBy(t => t.Departamento)
                .ToDictionaryAsync(g => g.Key, g => g.Count());

            foreach (var item in ticketsPorDepto)
            {
                estadisticas[$"Tickets_{item.Key}"] = item.Value;
            }

            return estadisticas;
        }

        public async Task<int> GetTotalEmpleadosPorDepartamentoAsync(byte departamentoId)
        {
            // Esta consulta necesitaría una relación directa entre empleados y departamentos
            // Por ahora retornamos 0, mDepartamentos -> dEmpleados -> mEmpleados -> mPerEmp -> mPersonas
            return 0;
        }

        public async Task<int> GetTotalTicketsPorDepartamentoAsync(byte departamentoId)
        {
            return await _legacyContext.TicketVistas
                .CountAsync(t => t.IdDepto == departamentoId);
        }
    }
}
