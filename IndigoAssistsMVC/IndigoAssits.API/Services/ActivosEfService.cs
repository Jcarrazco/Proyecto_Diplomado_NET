using IndigoAssits.API.Data;
using IndigoAssits.Core.Dtos;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssitsReglasDeNegocio.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IndigoAssits.API.Services
{
    public sealed class ActivosEfService : IActivoService
    {
        private readonly ActivosDbContext _context;

        public ActivosEfService(ActivosDbContext context)
        {
            _context = context;
        }

        public async Task<ActivoPaginadoDto> GetActivosPaginadosAsync(ActivoFiltroDto filtros)
        {
            var query = BuildFilterQuery(filtros);

            var total = await query.CountAsync();
            var page = filtros.Pagina <= 0 ? 1 : filtros.Pagina;
            var pageSize = filtros.TamanoPagina <= 0 ? 10 : filtros.TamanoPagina;
            var totalPaginas = (int)Math.Ceiling((double)total / pageSize);

            var items = await query
                .OrderByDescending(a => a.FeAlta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ActivoPaginadoDto
            {
                Items = items.Select(MapActivoToDto).ToList(),
                TotalRegistros = total,
                PaginaActual = page,
                TotalPaginas = totalPaginas,
                TamanoPagina = pageSize
            };
        }

        public async Task<IEnumerable<ActivoDto>> GetActivosAsync(ActivoFiltroDto filtros)
        {
            var items = await BuildFilterQuery(filtros)
                .OrderByDescending(a => a.FeAlta)
                .ToListAsync();

            return items.Select(MapActivoToDto);
        }

        public async Task<ActivoDto?> GetPorIdAsync(int idActivo)
        {
            var activo = await _context.Activos
                .Include(a => a.TipoActivo)
                .Include(a => a.Departamento)
                .Include(a => a.Status)
                .Include(a => a.Proveedor)
                .FirstOrDefaultAsync(a => a.IdActivo == idActivo);

            return activo == null ? null : MapActivoToDto(activo);
        }

        public async Task<int> CrearAsync(ActivoCreateDto dto)
        {
            var entidad = new Activo
            {
                Codigo = dto.Codigo,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Serie = dto.Serie,
                Nombre = dto.Nombre,
                PersonaAsign = dto.PersonaAsign,
                Ubicacion = dto.Ubicacion,
                FeAlta = dto.FeAlta,
                FeCompra = dto.FeCompra,
                FeBaja = dto.FeBaja,
                CostoCompra = dto.CostoCompra,
                Notas = dto.Notas,
                CodificacionComponentes = dto.CodificacionComponentes,
                TieneSoftwareOP = dto.TieneSoftwareOP,
                IdTipoActivo = dto.IdTipoActivo,
                IdDepartamento = dto.IdDepartamento,
                IdStatus = dto.IdStatus,
                IdProveedor = dto.IdProveedor
            };

            _context.Activos.Add(entidad);
            await _context.SaveChangesAsync();
            return entidad.IdActivo;
        }

        public async Task<bool> ActualizarAsync(ActivoUpdateDto dto)
        {
            var entidad = await _context.Activos.FirstOrDefaultAsync(a => a.IdActivo == dto.IdActivo);
            if (entidad == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(dto.Codigo)) entidad.Codigo = dto.Codigo;
            if (!string.IsNullOrWhiteSpace(dto.Marca)) entidad.Marca = dto.Marca;
            if (!string.IsNullOrWhiteSpace(dto.Modelo)) entidad.Modelo = dto.Modelo;
            if (!string.IsNullOrWhiteSpace(dto.Serie)) entidad.Serie = dto.Serie;
            if (!string.IsNullOrWhiteSpace(dto.Nombre)) entidad.Nombre = dto.Nombre;
            entidad.PersonaAsign = dto.PersonaAsign;
            entidad.Ubicacion = dto.Ubicacion;
            entidad.FeAlta = dto.FeAlta;
            entidad.FeCompra = dto.FeCompra;
            entidad.FeBaja = dto.FeBaja;
            entidad.CostoCompra = dto.CostoCompra;
            entidad.Notas = dto.Notas;
            entidad.CodificacionComponentes = dto.CodificacionComponentes;
            entidad.TieneSoftwareOP = dto.TieneSoftwareOP;
            entidad.IdTipoActivo = dto.IdTipoActivo;
            entidad.IdDepartamento = dto.IdDepartamento;
            entidad.IdStatus = dto.IdStatus;
            entidad.IdProveedor = dto.IdProveedor;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int idActivo)
        {
            var entidad = await _context.Activos.FirstOrDefaultAsync(a => a.IdActivo == idActivo);
            if (entidad == null)
            {
                return false;
            }

            _context.Activos.Remove(entidad);
            await _context.SaveChangesAsync();
            return true;
        }

        private IQueryable<Activo> BuildFilterQuery(ActivoFiltroDto filtros)
        {
            var query = _context.Activos
                .Include(a => a.TipoActivo)
                .Include(a => a.Departamento)
                .Include(a => a.Status)
                .Include(a => a.Proveedor)
                .AsQueryable();

            if (filtros.IdActivo.HasValue)
                query = query.Where(a => a.IdActivo == filtros.IdActivo.Value);

            if (!string.IsNullOrWhiteSpace(filtros.CodigoLike))
                query = query.Where(a => a.Codigo.Contains(filtros.CodigoLike));

            if (!string.IsNullOrWhiteSpace(filtros.MarcaLike))
                query = query.Where(a => a.Marca != null && a.Marca.Contains(filtros.MarcaLike));

            if (!string.IsNullOrWhiteSpace(filtros.NombreLike))
                query = query.Where(a => a.Nombre.Contains(filtros.NombreLike));

            if (!string.IsNullOrWhiteSpace(filtros.PersonaAsignLike))
                query = query.Where(a => a.PersonaAsign != null && a.PersonaAsign.Contains(filtros.PersonaAsignLike));

            if (!string.IsNullOrWhiteSpace(filtros.UbicacionLike))
                query = query.Where(a => a.Ubicacion != null && a.Ubicacion.Contains(filtros.UbicacionLike));

            if (filtros.TipoActivoId.HasValue)
                query = query.Where(a => a.IdTipoActivo == filtros.TipoActivoId.Value);

            if (filtros.DepartamentoId.HasValue)
                query = query.Where(a => a.IdDepartamento == filtros.DepartamentoId.Value);

            if (filtros.StatusId.HasValue)
                query = query.Where(a => a.IdStatus == filtros.StatusId.Value);

            if (filtros.ProveedorId.HasValue)
                query = query.Where(a => a.IdProveedor == filtros.ProveedorId.Value);

            if (filtros.TieneSoftwareOP.HasValue)
                query = query.Where(a => a.TieneSoftwareOP == filtros.TieneSoftwareOP.Value);

            if (filtros.CostoMin.HasValue)
                query = query.Where(a => a.CostoCompra >= filtros.CostoMin.Value);

            if (filtros.CostoMax.HasValue)
                query = query.Where(a => a.CostoCompra <= filtros.CostoMax.Value);

            if (filtros.FechaAltaDesde.HasValue)
                query = query.Where(a => a.FeAlta >= filtros.FechaAltaDesde.Value);

            if (filtros.FechaAltaHasta.HasValue)
                query = query.Where(a => a.FeAlta <= filtros.FechaAltaHasta.Value);

            if (filtros.FechaCompraDesde.HasValue)
                query = query.Where(a => a.FeCompra >= filtros.FechaCompraDesde.Value);

            if (filtros.FechaCompraHasta.HasValue)
                query = query.Where(a => a.FeCompra <= filtros.FechaCompraHasta.Value);

            if (filtros.FechaBajaDesde.HasValue)
                query = query.Where(a => a.FeBaja >= filtros.FechaBajaDesde.Value);

            if (filtros.FechaBajaHasta.HasValue)
                query = query.Where(a => a.FeBaja <= filtros.FechaBajaHasta.Value);

            if (filtros.ComponentesSeleccionados != null && filtros.ComponentesSeleccionados.Count > 0)
            {
                var mask = filtros.ComponentesSeleccionados.Sum();
                query = query.Where(a => ((a.CodificacionComponentes ?? 0) & mask) != 0);
            }

            return query;
        }

        private static ActivoDto MapActivoToDto(Activo a)
        {
            return new ActivoDto
            {
                IdActivo = a.IdActivo,
                Codigo = a.Codigo,
                Marca = a.Marca,
                Modelo = a.Modelo,
                Serie = a.Serie,
                Nombre = a.Nombre,
                PersonaAsign = a.PersonaAsign,
                Ubicacion = a.Ubicacion,
                FeAlta = a.FeAlta,
                FeCompra = a.FeCompra,
                FeBaja = a.FeBaja,
                CostoCompra = a.CostoCompra,
                Notas = a.Notas,
                CodificacionComponentes = a.CodificacionComponentes ?? 0,
                TieneSoftwareOP = a.TieneSoftwareOP ?? false,
                IdTipoActivo = a.IdTipoActivo,
                IdDepartamento = a.IdDepartamento,
                IdStatus = a.IdStatus,
                IdProveedor = a.IdProveedor,
                TipoActivoNombre = a.TipoActivo?.TipoActivoNombre,
                DepartamentoNombre = a.Departamento?.Departamento,
                StatusNombre = a.Status?.StatusNombre,
                ProveedorNombre = a.Proveedor?.ProveedorNombre
            };
        }
    }
}
