namespace ActivosApp.Models;

public class UserContextDto
{
    public int IdPersona { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Paterno { get; set; } = string.Empty;
    public string Materno { get; set; } = string.Empty;
    public byte? IdDepto { get; set; }
    public string Departamento { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public string Sucursal { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
}
