using CoreWCF;
using IndigoAssits.Core.Dtos;

namespace IndigoAssist.NetCoreWCF.Services
{
    [ServiceContract]
    public interface ICategoriasServiceWCF
    {
        [OperationContract]
        List<CategoriaDto> ObtenerTodos();
    
    }
}
