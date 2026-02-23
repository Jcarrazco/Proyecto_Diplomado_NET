using IndigoAssits.Core.Dtos;
using IndigoAssitsReglasDeNegocio.Interfaces;

namespace IndigoAssist.NetCoreWCF.Services
{
    public class CategoriasServiceWCF : ICategoriasServiceWCF
    {
        private readonly ICategoriaService categoriaService;

        public CategoriasServiceWCF(ICategoriaService categoriaService) 
        {
            this.categoriaService = categoriaService;
        }

        public List<CategoriaDto> ObtenerTodos()
        {
            List<CategoriaDto> lista =  this.categoriaService.GetCategoriasAsync().Result.ToList();

            return lista;
        }
    }
}
