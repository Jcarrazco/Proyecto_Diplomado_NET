using IndigoAssits.Core.Dtos;
using IndigoAssits.Repositorio.Core.Interfaces;
using IndigoAssitsReglasDeNegocio.Interfaces;

namespace IndigoAssitsReglasDeNegocio.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoriaDto>> GetCategoriasAsync()
        {
            var cats = await _unitOfWork.Categorias.GetCategoriasAsync();
            return cats.Select(c => new CategoriaDto { IdCategoria = c.IdCategoria, Categoria = c.Categoria });
        }

        public async Task<IEnumerable<CategoriaDto>> GetCategoriasPorDepartamentoAsync(byte idDepartamento)
        {
            var cats = await _unitOfWork.Categorias.GetCategoriasByDepartamentoAsync(idDepartamento);
            return cats.Select(c => new CategoriaDto { IdCategoria = c.IdCategoria, Categoria = c.Categoria });
        }
    }
}


