namespace ActivosApp.Models;

public class TicketResponseDto
{
    public int IdTicket { get; set; }
    public int UsuarioSolicitante { get; set; }
    public string SolicitanteNombre { get; set; } = string.Empty;
    public byte IdSubCategoria { get; set; }
    public string SubCategoriaNombre { get; set; } = string.Empty;
    public byte IdCategoria { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public byte Status { get; set; }
    public string StatusDescripcion { get; set; } = string.Empty;
    public byte IdTipoTicket { get; set; }
    public string TipoTicketNombre { get; set; } = string.Empty;
    public byte Prioridad { get; set; }
    public string PrioridadNombre { get; set; } = string.Empty;
    public DateTime FeAlta { get; set; }
    public DateTime FeAsignacion { get; set; }
    public DateTime FeCompromiso { get; set; }
    public DateTime FeCierre { get; set; }
    public int IdTecnico { get; set; }
    public string TecnicoNombre { get; set; } = string.Empty;
    public byte IdDepartamento { get; set; }
    public string DepartamentoNombre { get; set; } = string.Empty;
}
