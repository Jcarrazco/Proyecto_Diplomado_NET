using System.Linq;

namespace IndigoAssits.Core.Dtos
{
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

        public string NombreCompleto
        {
            get
            {
                var full = string.Join(" ", new[] { Nombre, Paterno, Materno }.Where(s => !string.IsNullOrWhiteSpace(s)));
                return string.IsNullOrWhiteSpace(full) ? Login : full;
            }
        }
    }
}
