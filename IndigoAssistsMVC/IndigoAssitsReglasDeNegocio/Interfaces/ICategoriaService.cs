using IndigoAssits.Core.Dtos;

namespace IndigoAssitsReglasDeNegocio.Interfaces
{
    public interface ICategoriaService
    {
        Task<IEnumerable<CategoriaDto>> GetCategoriasAsync();
        Task<IEnumerable<CategoriaDto>> GetCategoriasPorDepartamentoAsync(byte idDepartamento);
    }
}

