using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAsists.Repositorio.Db;

namespace IndigoAsists.Repositorio.Repositories
{
    public class EmpleadoRepository : GenericRepository<mEmpleados>, IEmpleadoRepository
    {
        private readonly IndigoLegacyDbContext _legacyContext;

        public EmpleadoRepository(IndigoLegacyDbContext context) : base(context)
        {
            _legacyContext = context;
        }

        public async Task<IEnumerable<mEmpleados>> GetEmpleadosTecnicosAsync()
        {
             var tecnicosIds = await _legacyContext.TicketsTecnicos
                .Select(tt => tt.IdPersona)
                .Distinct()
                .ToListAsync();

            return await _dbSet
                .Where(e => tecnicosIds.Contains(e.IdPersona))
                .Include(e => e.PersonaEmpresa)
                .ToListAsync();
        }

        public async Task<IEnumerable<mEmpleados>> GetEmpleadosByDepartamentoAsync(byte departamentoId)
        {
            // Esta consulta necesitaría una relación directa entre empleados y departamentos
            // Por ahora retornamos todos los empleados activos
            return await _dbSet
                .Where(e => e.Activo)
                .Include(e => e.PersonaEmpresa)
                .ToListAsync();
        }

        public async Task<mEmpleados?> GetEmpleadoWithPersonaAsync(int idPersona)
        {
            return await _dbSet
                .Where(e => e.IdPersona == idPersona)
                .Include(e => e.PersonaEmpresa)
                .ThenInclude(pe => pe.PersonaInfo)
                .FirstOrDefaultAsync();
        }

        public async Task<mEmpleados?> GetEmpleadoByLoginAsync(string login)
        {
            return await _dbSet
                .Where(e => e.Login == login && e.Activo)
                .Include(e => e.PersonaEmpresa)
                .ThenInclude(pe => pe.PersonaInfo)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<mEmpleados>> GetEmpleadosWithPersonaAsync()
        {
            return await _dbSet
                .Where(e => e.Activo)
                .Include(e => e.PersonaEmpresa)
                .ThenInclude(pe => pe.PersonaInfo)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetEstadisticasEmpleadosAsync()
        {
            var estadisticas = new Dictionary<string, int>();

            // Total de empleados
            estadisticas["TotalEmpleados"] = await _dbSet.CountAsync();

            // Empleados activos
            estadisticas["EmpleadosActivos"] = await _dbSet.CountAsync(e => e.Activo);

            // Empleados técnicos (que tienen tickets asignados)
            var tecnicosIds = await _legacyContext.TicketsTecnicos
                .Select(tt => tt.IdPersona)
                .Distinct()
                .ToListAsync();

            estadisticas["EmpleadosTecnicos"] = await _dbSet
                .CountAsync(e => tecnicosIds.Contains(e.IdPersona));

            return estadisticas;
        }

        public async Task<int> GetTotalEmpleadosActivosAsync()
        {
            return await _dbSet.CountAsync(e => e.Activo);
        }

        public async Task<int> GetTotalEmpleadosByDepartamentoAsync(byte departamentoId)
        {
            // Esta consulta necesitaría una relación directa entre empleados y departamentos
            // Por ahora retornamos el total de empleados activos
            return await _dbSet.CountAsync(e => e.Activo);
        }
    }
}
