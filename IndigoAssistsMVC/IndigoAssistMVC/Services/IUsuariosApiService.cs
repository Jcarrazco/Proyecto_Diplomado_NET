using IndigoAssits.Core.Dtos;

namespace IndigoAssistMVC.Services
{
    public interface IUsuariosApiService
    {
        Task<bool> EnsureLegacyUserAsync(string email, string legacyLogin, byte? idDepto);
    }
}
