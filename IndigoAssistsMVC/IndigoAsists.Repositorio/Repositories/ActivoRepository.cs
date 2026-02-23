using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAsists.Repositorio.Db;

namespace IndigoAsists.Repositorio.Repositories
{
    public class ActivoRepository : GenericRepository<Activo>, IActivoRepository
    {
        public ActivoRepository(IndigoLegacyDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Activo> Items, int TotalCount)> GetActivosPagedAsync(
            int page,
            int pageSize,
            int? idActivo = null,
            string? codigoLike = null,
            string? marcaLike = null,
            string? nombreLike = null,
            string? personaAsignLike = null,
            string? ubicacionLike = null,
            byte? tipoActivoId = null,
            byte? departamentoId = null,
            byte? statusId = null,
            byte? proveedorId = null,
            bool? tieneSoftwareOP = null,
            decimal? costoMin = null,
            decimal? costoMax = null,
            DateTime? fechaAltaDesde = null,
            DateTime? fechaAltaHasta = null,
            DateTime? fechaCompraDesde = null,
            DateTime? fechaCompraHasta = null,
            DateTime? fechaBajaDesde = null,
            DateTime? fechaBajaHasta = null,
            int? componentesMask = null)
        {
            var query = BuildFilterQuery(idActivo, codigoLike, marcaLike, nombreLike, personaAsignLike, ubicacionLike,
                tipoActivoId, departamentoId, statusId, proveedorId, tieneSoftwareOP, costoMin, costoMax,
                fechaAltaDesde, fechaAltaHasta, fechaCompraDesde, fechaCompraHasta, fechaBajaDesde, fechaBajaHasta, componentesMask);

            var totalCount = await query.CountAsync();
            var items = await query
                .Include(a => a.TipoActivo)
                .Include(a => a.Departamento)
                .Include(a => a.Status)
                .Include(a => a.Proveedor)
                .OrderByDescending(a => a.FeAlta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Activo>> GetActivosWithFiltersAsync(
            int? idActivo = null,
            string? codigoLike = null,
            string? marcaLike = null,
            string? nombreLike = null,
            string? personaAsignLike = null,
            string? ubicacionLike = null,
            byte? tipoActivoId = null,
            byte? departamentoId = null,
            byte? statusId = null,
            byte? proveedorId = null,
            bool? tieneSoftwareOP = null,
            decimal? costoMin = null,
            decimal? costoMax = null,
            DateTime? fechaAltaDesde = null,
            DateTime? fechaAltaHasta = null,
            DateTime? fechaCompraDesde = null,
            DateTime? fechaCompraHasta = null,
            DateTime? fechaBajaDesde = null,
            DateTime? fechaBajaHasta = null,
            int? componentesMask = null)
        {
            var query = BuildFilterQuery(idActivo, codigoLike, marcaLike, nombreLike, personaAsignLike, ubicacionLike,
                tipoActivoId, departamentoId, statusId, proveedorId, tieneSoftwareOP, costoMin, costoMax,
                fechaAltaDesde, fechaAltaHasta, fechaCompraDesde, fechaCompraHasta, fechaBajaDesde, fechaBajaHasta, componentesMask);

            return await query
                .Include(a => a.TipoActivo)
                .Include(a => a.Departamento)
                .Include(a => a.Status)
                .Include(a => a.Proveedor)
                .OrderByDescending(a => a.FeAlta)
                .ToListAsync();
        }

        private IQueryable<Activo> BuildFilterQuery(
            int? idActivo = null,
            string? codigoLike = null,
            string? marcaLike = null,
            string? nombreLike = null,
            string? personaAsignLike = null,
            string? ubicacionLike = null,
            byte? tipoActivoId = null,
            byte? departamentoId = null,
            byte? statusId = null,
            byte? proveedorId = null,
            bool? tieneSoftwareOP = null,
            decimal? costoMin = null,
            decimal? costoMax = null,
            DateTime? fechaAltaDesde = null,
            DateTime? fechaAltaHasta = null,
            DateTime? fechaCompraDesde = null,
            DateTime? fechaCompraHasta = null,
            DateTime? fechaBajaDesde = null,
            DateTime? fechaBajaHasta = null,
            int? componentesMask = null)
        {
            var query = _dbSet.AsQueryable();

            if (idActivo.HasValue)
                query = query.Where(a => a.IdActivo == idActivo.Value);

            if (!string.IsNullOrWhiteSpace(codigoLike))
                query = query.Where(a => a.Codigo.Contains(codigoLike));

            if (!string.IsNullOrWhiteSpace(marcaLike))
                query = query.Where(a => a.Marca != null && a.Marca.Contains(marcaLike));

            if (!string.IsNullOrWhiteSpace(nombreLike))
                query = query.Where(a => a.Nombre.Contains(nombreLike));

            if (!string.IsNullOrWhiteSpace(personaAsignLike))
                query = query.Where(a => a.PersonaAsign != null && a.PersonaAsign.Contains(personaAsignLike));

            if (!string.IsNullOrWhiteSpace(ubicacionLike))
                query = query.Where(a => a.Ubicacion != null && a.Ubicacion.Contains(ubicacionLike));

            if (tipoActivoId.HasValue)
                query = query.Where(a => a.IdTipoActivo == tipoActivoId.Value);

            if (departamentoId.HasValue)
                query = query.Where(a => a.IdDepartamento == departamentoId.Value);

            if (statusId.HasValue)
                query = query.Where(a => a.IdStatus == statusId.Value);

            if (proveedorId.HasValue)
                query = query.Where(a => a.IdProveedor == proveedorId.Value);

            if (tieneSoftwareOP.HasValue)
                query = query.Where(a => a.TieneSoftwareOP == tieneSoftwareOP.Value);

            if (costoMin.HasValue)
                query = query.Where(a => a.CostoCompra >= costoMin.Value);

            if (costoMax.HasValue)
                query = query.Where(a => a.CostoCompra <= costoMax.Value);

            if (fechaAltaDesde.HasValue)
                query = query.Where(a => a.FeAlta >= fechaAltaDesde.Value);

            if (fechaAltaHasta.HasValue)
                query = query.Where(a => a.FeAlta <= fechaAltaHasta.Value);

            if (fechaCompraDesde.HasValue)
                query = query.Where(a => a.FeCompra >= fechaCompraDesde.Value);

            if (fechaCompraHasta.HasValue)
                query = query.Where(a => a.FeCompra <= fechaCompraHasta.Value);

            if (fechaBajaDesde.HasValue)
                query = query.Where(a => a.FeBaja >= fechaBajaDesde.Value);

            if (fechaBajaHasta.HasValue)
                query = query.Where(a => a.FeBaja <= fechaBajaHasta.Value);

            if (componentesMask.HasValue && componentesMask.Value > 0)
                query = query.Where(a => ((a.CodificacionComponentes ?? 0) & componentesMask.Value) != 0);

            return query;
        }
    }
}

