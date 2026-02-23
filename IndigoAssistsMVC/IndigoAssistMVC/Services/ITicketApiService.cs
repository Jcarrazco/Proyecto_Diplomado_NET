using IndigoAssits.Core.Dtos;
using IndigoAssistMVC.Models;

namespace IndigoAssistMVC.Services
{
    public interface ITicketApiService
    {
        Task<List<TicketVista>> GetTicketsAsync(TicketFiltroDto? filtros = null);
        Task<List<TicketVista>> GetTicketsAbiertosAsync(TicketFiltroDto? filtros = null);
        Task<TicketResponseDto?> GetTicketByIdAsync(int idTicket);
        Task<UserContextDto?> GetUsuarioContextoAsync();
        Task<TicketDashboardDto?> GetDashboardAsync(string scope);
        Task<List<TecnicoDto>> GetTecnicosAsync(byte? departamentoId = null);
        Task<bool> AsignarTecnicoAsync(int idTicket, int tecnicoId, DateTime? fechaCompromiso = null);
    }
}
