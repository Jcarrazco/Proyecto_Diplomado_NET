using IndigoAssits.Repositorio.Core.Entities;

namespace IndigoAssits.Repositorio.Core.Interfaces
{
    public interface IActivoRepository : IGenericRepository<Activo>
    {
        Task<(IEnumerable<Activo> Items, int TotalCount)> GetActivosPagedAsync(
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
            int? componentesMask = null);

        Task<IEnumerable<Activo>> GetActivosWithFiltersAsync(
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
            int? componentesMask = null);
    }
}

