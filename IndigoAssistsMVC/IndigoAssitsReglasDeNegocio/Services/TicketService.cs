using IndigoAssits.Core.Dtos;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAssitsReglasDeNegocio.Interfaces;

namespace IndigoAssitsReglasDeNegocio.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TicketPaginadoDto> GetTicketsPaginadosAsync(TicketFiltroDto filtros)
        {
            var status = ResolveStatusFilter(filtros);
            var (items, total) = await _unitOfWork.Tickets.GetTicketsPagedAsync(
                page: filtros.Pagina,
                pageSize: filtros.TamañoPagina,
                usuarioId: filtros.UsuarioSolicitante == 0 ? null : filtros.UsuarioSolicitante,
                tecnicoId: filtros.IdTecnico == 0 ? null : filtros.IdTecnico,
                estado: status,
                prioridad: filtros.Prioridad == 0 ? null : filtros.Prioridad,
                categoriaId: filtros.IdCategoria == 0 ? null : filtros.IdCategoria,
                departamentoId: filtros.IdDepartamento == 0 ? null : filtros.IdDepartamento,
                fechaInicio: filtros.FechaInicio == default ? null : filtros.FechaInicio,
                fechaFin: filtros.FechaFin == default ? null : filtros.FechaFin,
                busquedaTexto: string.IsNullOrWhiteSpace(filtros.BusquedaTexto) ? null : filtros.BusquedaTexto
            );

            var lista = items.Select(MapTicketVistaToDto).ToList();
            var totalPaginas = filtros.TamañoPagina == 0 ? 1 : (int)Math.Ceiling((double)total / filtros.TamañoPagina);

            return new TicketPaginadoDto
            {
                Tickets = lista,
                TotalRegistros = total,
                PaginaActual = filtros.Pagina,
                TotalPaginas = totalPaginas,
                TamañoPagina = filtros.TamañoPagina
            };
        }

        public async Task<IEnumerable<TicketResponseDto>> GetTicketsAsync(TicketFiltroDto filtros)
        {
            var status = ResolveStatusFilter(filtros);
            var items = await _unitOfWork.Tickets.GetTicketsWithFiltersAsync(
                usuarioId: filtros.UsuarioSolicitante == 0 ? null : filtros.UsuarioSolicitante,
                tecnicoId: filtros.IdTecnico == 0 ? null : filtros.IdTecnico,
                estado: status,
                prioridad: filtros.Prioridad == 0 ? null : filtros.Prioridad,
                categoriaId: filtros.IdCategoria == 0 ? null : filtros.IdCategoria,
                departamentoId: filtros.IdDepartamento == 0 ? null : filtros.IdDepartamento,
                fechaInicio: filtros.FechaInicio == default ? null : filtros.FechaInicio,
                fechaFin: filtros.FechaFin == default ? null : filtros.FechaFin,
                busquedaTexto: string.IsNullOrWhiteSpace(filtros.BusquedaTexto) ? null : filtros.BusquedaTexto
            );

            return items.Select(MapTicketVistaToDto);
        }

        public async Task<TicketResponseDto?> GetTicketPorIdAsync(int idTicket)
        {
            var vista = (await _unitOfWork.Tickets.GetTicketsWithFiltersAsync()).FirstOrDefault(t => t.IdTicket == idTicket);
            return vista == null ? null : MapTicketVistaToDto(vista);
        }

        public async Task<int> CrearTicketAsync(TicketCreateDto dto)
        {
            var entidad = new Ticket
            {
                Usuario = dto.UsuarioSolicitante,
                IdSubCategoria = dto.IdSubCategoria,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Status = 1,
                IdTipoTicket = dto.IdTipoTicket,
                Prioridad = dto.Prioridad,
                FeAlta = DateTime.UtcNow
            };

            await _unitOfWork.Tickets.AddAsync(entidad);
            await _unitOfWork.SaveChangesAsync();
            return entidad.IdTicket;
        }

        public async Task<bool> ActualizarTicketAsync(TicketUpdateDto dto)
        {
            var entidad = await _unitOfWork.Tickets.GetByIdAsync(dto.IdTicket);
            if (entidad == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.Titulo)) entidad.Titulo = dto.Titulo;
            if (!string.IsNullOrWhiteSpace(dto.Descripcion)) entidad.Descripcion = dto.Descripcion;
            if (dto.Status.HasValue) entidad.Status = dto.Status.Value;
            if (dto.IdTipoTicket.HasValue) entidad.IdTipoTicket = dto.IdTipoTicket.Value;
            if (dto.Prioridad.HasValue) entidad.Prioridad = dto.Prioridad.Value;
            if (dto.IdSubCategoria.HasValue) entidad.IdSubCategoria = dto.IdSubCategoria.Value;
            if (dto.FeAsignacion.HasValue) entidad.FeAsignacion = dto.FeAsignacion.Value;
            if (dto.FeCompromiso.HasValue) entidad.FeCompromiso = dto.FeCompromiso.Value;
            if (dto.FeCierre.HasValue) entidad.FeCierre = dto.FeCierre.Value;

            await _unitOfWork.Tickets.UpdateAsync(entidad);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AsignarTicketAsync(TicketAsignacionDto dto)
        {
            return await _unitOfWork.Tickets.AsignarTicketAsync(dto.IdTicket, dto.IdTecnico, dto.FeCompromiso);
        }

        public async Task<bool> AsignarTicketAsync(TicketAsignacionMultipleDto dto)
        {
            if (dto.Tecnicos == null || dto.Tecnicos.Count == 0)
            {
                return false;
            }

            var ok = true;
            foreach (var tecnicoId in dto.Tecnicos)
            {
                var result = await _unitOfWork.Tickets.AsignarTicketAsync(dto.IdTicket, tecnicoId, dto.FeCompromiso);
                if (!result)
                {
                    ok = false;
                }
            }

            return ok;
        }

        public Task<bool> AgregarAnotacionAsync(TicketAnotacionCreateDto dto)
        {
            return Task.FromResult(false);
        }

        public async Task<bool> CerrarTicketAsync(int idTicket)
        {
            return await CambiarEstadoTicketAsync(idTicket, 3);
        }

        public async Task<bool> ReabrirTicketAsync(int idTicket)
        {
            return await CambiarEstadoTicketAsync(idTicket, 2);
        }

        public async Task<bool> DesasignarTicketAsync(int idTicket)
        {
            return await _unitOfWork.Tickets.DesasignarTicketAsync(idTicket);
        }

        public async Task<bool> CambiarEstadoTicketAsync(int idTicket, byte nuevoEstado)
        {
            return await _unitOfWork.Tickets.CambiarEstadoTicketAsync(idTicket, nuevoEstado);
        }

        public async Task<TicketEstadisticasDto> GetEstadisticasAsync(byte? idDepartamento = null)
        {
            var dto = new TicketEstadisticasDto();
            dto.TotalAbiertos = idDepartamento.HasValue ?
                await _unitOfWork.Tickets.GetTotalTicketsAbiertosByDepartamentoAsync(idDepartamento.Value) :
                (await _unitOfWork.Tickets.GetTicketsByEstadoAsync(1)).Count();

            dto.TotalEnProceso = idDepartamento.HasValue ?
                await _unitOfWork.Tickets.GetTotalTicketsEnProcesoByDepartamentoAsync(idDepartamento.Value) :
                (await _unitOfWork.Tickets.GetTicketsByEstadoAsync(2)).Count();

            dto.TotalCerrados = idDepartamento.HasValue ?
                await _unitOfWork.Tickets.GetTotalTicketsCerradosByDepartamentoAsync(idDepartamento.Value) :
                (await _unitOfWork.Tickets.GetTicketsByEstadoAsync(3)).Count();

            dto.PorDepartamento = await _unitOfWork.Tickets.GetEstadisticasPorDepartamentoAsync();
            dto.PorPrioridad = await _unitOfWork.Tickets.GetEstadisticasPorPrioridadAsync();
            dto.PorEstado = await _unitOfWork.Tickets.GetEstadisticasPorEstadoAsync();

            return dto;
        }

        public async Task<IEnumerable<TicketResponseDto>> GetTicketsRecientesAsync(int cantidad = 10)
        {
            var items = await _unitOfWork.Tickets.GetTicketsRecientesAsync(cantidad);
            return items.Select(MapTicketVistaToDto);
        }

        public async Task<IEnumerable<TicketResponseDto>> BuscarTicketsAsync(string terminoBusqueda)
        {
            var items = await _unitOfWork.Tickets.BuscarTicketsAsync(terminoBusqueda);
            return items.Select(MapTicketVistaToDto);
        }

        private static TicketResponseDto MapTicketVistaToDto(TicketVista t)
        {
            return new TicketResponseDto
            {
                IdTicket = t.IdTicket,
                UsuarioSolicitante = t.IdSolicitante,
                SolicitanteNombre = t.Solicitante,
                IdSubCategoria = t.IdSubCategoria,
                SubCategoriaNombre = t.SubCategoria,
                IdCategoria = t.IdCategoria,
                CategoriaNombre = t.Categoria,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Status = t.Status,
                StatusDescripcion = t.StatusDes,
                IdTipoTicket = (byte)(t.IdTipoTicket ?? 0),
                Prioridad = (byte)(t.IdPrioridad ?? 0),
                PrioridadNombre = t.IdPrioridad?.ToString() ?? string.Empty,
                FeAlta = t.FeAlta,
                FeAsignacion = t.FeAsignacion ?? default,
                FeCompromiso = t.FeCompromiso ?? default,
                FeCierre = t.FeCierre ?? default,
                IdTecnico = t.IdTecnico ?? 0,
                TecnicoNombre = t.Tecnico,
                IdDepartamento = t.IdDepto,
                DepartamentoNombre = t.Departamento
            };
        }

        private static byte? ResolveStatusFilter(TicketFiltroDto filtros)
        {
            if (filtros.StatusId.HasValue)
            {
                return filtros.StatusId.Value;
            }

            if (string.IsNullOrWhiteSpace(filtros.Status))
            {
                return null;
            }

            if (byte.TryParse(filtros.Status, out var numericStatus))
            {
                return numericStatus;
            }

            var normalized = filtros.Status.Trim().ToLowerInvariant();
            return normalized switch
            {
                "nuevo" => (byte)1,
                "abierto" => (byte)1,
                "asignado" => (byte)2,
                "enproceso" => (byte)2,
                "en_proceso" => (byte)2,
                "cerrado" => (byte)3,
                "todos" => (byte)0,
                _ => null
            };
        }
    }
}


