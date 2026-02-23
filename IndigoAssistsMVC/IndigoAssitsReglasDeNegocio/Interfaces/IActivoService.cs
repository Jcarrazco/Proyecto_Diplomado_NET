using IndigoAssits.Core.Dtos;

namespace IndigoAssitsReglasDeNegocio.Interfaces
{
    public interface IActivoService
    {
        Task<ActivoPaginadoDto> GetActivosPaginadosAsync(ActivoFiltroDto filtros);
        Task<IEnumerable<ActivoDto>> GetActivosAsync(ActivoFiltroDto filtros);
        Task<ActivoDto?> GetPorIdAsync(int idActivo);
        Task<int> CrearAsync(ActivoCreateDto dto);
        Task<bool> ActualizarAsync(ActivoUpdateDto dto);
        Task<bool> EliminarAsync(int idActivo);
    }
}

