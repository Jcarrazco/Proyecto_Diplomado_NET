using IndigoAssits.Core.Dtos;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAssitsReglasDeNegocio.Interfaces;

namespace IndigoAssitsReglasDeNegocio.Services
{
    public class ActivoService : IActivoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivoPaginadoDto> GetActivosPaginadosAsync(ActivoFiltroDto filtros)
        {
            int componentesMask = filtros.ComponentesSeleccionados.Any() 
                                            ? filtros.ComponentesSeleccionados.Sum() 
                                            : 0 ;

            var (items, total) = await _unitOfWork.Activos.GetActivosPagedAsync(
                page: filtros.Pagina,
                pageSize: filtros.TamanoPagina,
                idActivo: filtros.IdActivo,
                codigoLike: string.IsNullOrWhiteSpace(filtros.CodigoLike) ? null : filtros.CodigoLike,
                marcaLike: string.IsNullOrWhiteSpace(filtros.MarcaLike) ? null : filtros.MarcaLike,
                nombreLike: string.IsNullOrWhiteSpace(filtros.NombreLike) ? null : filtros.NombreLike,
                personaAsignLike: string.IsNullOrWhiteSpace(filtros.PersonaAsignLike) ? null : filtros.PersonaAsignLike,
                ubicacionLike: string.IsNullOrWhiteSpace(filtros.UbicacionLike) ? null : filtros.UbicacionLike,
                tipoActivoId: filtros.TipoActivoId,
                departamentoId: filtros.DepartamentoId,
                statusId: filtros.StatusId,
                proveedorId: filtros.ProveedorId,
                tieneSoftwareOP: filtros.TieneSoftwareOP,
                costoMin: filtros.CostoMin,
                costoMax: filtros.CostoMax,
                fechaAltaDesde: filtros.FechaAltaDesde,
                fechaAltaHasta: filtros.FechaAltaHasta,
                fechaCompraDesde: filtros.FechaCompraDesde,
                fechaCompraHasta: filtros.FechaCompraHasta,
                fechaBajaDesde: filtros.FechaBajaDesde,
                fechaBajaHasta: filtros.FechaBajaHasta,
                componentesMask: componentesMask
            );

            var lista = items.Select(MapActivoToDto).ToList();
            var totalPaginas = filtros.TamanoPagina == 0 ? 1 : (int)Math.Ceiling((double)total / filtros.TamanoPagina);

            return new ActivoPaginadoDto
            {
                Items = lista,
                TotalRegistros = total,
                PaginaActual = filtros.Pagina,
                TotalPaginas = totalPaginas,
                TamanoPagina = filtros.TamanoPagina
            };
        }

        public async Task<IEnumerable<ActivoDto>> GetActivosAsync(ActivoFiltroDto filtros)
        {
            int componentesMask = filtros.ComponentesSeleccionados?.Any() == true
                ? filtros.ComponentesSeleccionados.Sum() 
                : 0;

            var items = await _unitOfWork.Activos.GetActivosWithFiltersAsync(
                idActivo: filtros.IdActivo,
                codigoLike: string.IsNullOrWhiteSpace(filtros.CodigoLike) ? null : filtros.CodigoLike,
                marcaLike: string.IsNullOrWhiteSpace(filtros.MarcaLike) ? null : filtros.MarcaLike,
                nombreLike: string.IsNullOrWhiteSpace(filtros.NombreLike) ? null : filtros.NombreLike,
                personaAsignLike: string.IsNullOrWhiteSpace(filtros.PersonaAsignLike) ? null : filtros.PersonaAsignLike,
                ubicacionLike: string.IsNullOrWhiteSpace(filtros.UbicacionLike) ? null : filtros.UbicacionLike,
                tipoActivoId: filtros.TipoActivoId,
                departamentoId: filtros.DepartamentoId,
                statusId: filtros.StatusId,
                proveedorId: filtros.ProveedorId,
                tieneSoftwareOP: filtros.TieneSoftwareOP,
                costoMin: filtros.CostoMin,
                costoMax: filtros.CostoMax,
                fechaAltaDesde: filtros.FechaAltaDesde,
                fechaAltaHasta: filtros.FechaAltaHasta,
                fechaCompraDesde: filtros.FechaCompraDesde,
                fechaCompraHasta: filtros.FechaCompraHasta,
                fechaBajaDesde: filtros.FechaBajaDesde,
                fechaBajaHasta: filtros.FechaBajaHasta,
                componentesMask: componentesMask > 0 ? componentesMask : null
            );

            return items.Select(MapActivoToDto);
        }

        public async Task<ActivoDto?> GetPorIdAsync(int idActivo)
        {
            var activo = await _unitOfWork.Activos.GetByIdAsync(idActivo);
            if (activo == null) return null;
            return MapActivoToDto(activo);
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

            await _unitOfWork.Activos.AddAsync(entidad);
            await _unitOfWork.SaveChangesAsync();
            return entidad.IdActivo;
        }

        public async Task<bool> ActualizarAsync(ActivoUpdateDto dto)
        {
            var entidad = await _unitOfWork.Activos.GetByIdAsync(dto.IdActivo);
            if (entidad == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Codigo)) entidad.Codigo = dto.Codigo;
            if (!string.IsNullOrWhiteSpace(dto.Marca)) entidad.Marca = dto.Marca;
            if (!string.IsNullOrWhiteSpace(dto.Modelo)) entidad.Modelo = dto.Modelo;
            if (!string.IsNullOrWhiteSpace(dto.Serie)) entidad.Serie = dto.Serie;
            if (!string.IsNullOrWhiteSpace(dto.Nombre)) entidad.Nombre = dto.Nombre;
            if (dto.PersonaAsign != null) entidad.PersonaAsign = dto.PersonaAsign;
            if (dto.Ubicacion != null) entidad.Ubicacion = dto.Ubicacion;
            entidad.FeAlta = dto.FeAlta;
            if (dto.FeCompra.HasValue) entidad.FeCompra = dto.FeCompra;
            if (dto.FeBaja.HasValue) entidad.FeBaja = dto.FeBaja;
            if (dto.CostoCompra.HasValue) entidad.CostoCompra = dto.CostoCompra;
            if (dto.Notas != null) entidad.Notas = dto.Notas;
            entidad.CodificacionComponentes = dto.CodificacionComponentes;
            entidad.TieneSoftwareOP = dto.TieneSoftwareOP;
            if (dto.IdTipoActivo.HasValue) entidad.IdTipoActivo = dto.IdTipoActivo;
            if (dto.IdDepartamento.HasValue) entidad.IdDepartamento = dto.IdDepartamento;
            if (dto.IdStatus.HasValue) entidad.IdStatus = dto.IdStatus;
            if (dto.IdProveedor.HasValue) entidad.IdProveedor = dto.IdProveedor;

            await _unitOfWork.Activos.UpdateAsync(entidad);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int idActivo)
        {
            var entidad = await _unitOfWork.Activos.GetByIdAsync(idActivo);
            if (entidad == null) return false;

            await _unitOfWork.Activos.DeleteAsync(entidad);
            await _unitOfWork.SaveChangesAsync();
            return true;
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

