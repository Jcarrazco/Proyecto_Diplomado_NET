namespace ActivosApp.Models;

public class TicketAnotacionCreateDto
{
    public int IdTicket { get; set; }
    public string Observacion { get; set; } = string.Empty;
    public int Duracion { get; set; }
    public string Usuario { get; set; } = string.Empty;
}
