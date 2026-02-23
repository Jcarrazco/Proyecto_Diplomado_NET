namespace ActivosApp.Models;

public class TicketDashboardTotalesDto
{
    public int Abiertos { get; set; }
    public int EnProceso { get; set; }
    public int Asignados { get; set; }
    public int Cerrados { get; set; }
}

public class TicketDashboardListasDto
{
    public List<TicketResponseDto> AbiertosDepto { get; set; } = new List<TicketResponseDto>();
    public List<TicketResponseDto> EnProcesoDepto { get; set; } = new List<TicketResponseDto>();
    public List<TicketResponseDto> Asignados { get; set; } = new List<TicketResponseDto>();
    public List<TicketResponseDto> AbiertosUsuario { get; set; } = new List<TicketResponseDto>();
    public List<TicketResponseDto> EnProcesoUsuario { get; set; } = new List<TicketResponseDto>();
}

public class TicketDashboardDto
{
    public UserContextDto Contexto { get; set; } = new UserContextDto();
    public TicketDashboardTotalesDto Totales { get; set; } = new TicketDashboardTotalesDto();
    public TicketDashboardListasDto Listas { get; set; } = new TicketDashboardListasDto();
}
