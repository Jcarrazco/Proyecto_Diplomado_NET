using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoAssits.Core.Dtos
{
    public class CategoriaTicketDto
    {
        public byte IdCategoria { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public byte IdDepto { get; set; }
        public string DepartamentoNombre { get; set; } = string.Empty;
        public List<SubCategoriaTicketDto> SubCategorias { get; set; } = new List<SubCategoriaTicketDto>();
    }

    public class SubCategoriaTicketDto
    {
        public byte IdSubCategoria { get; set; }
        public string SubCategoria { get; set; } = string.Empty;
        public byte IdCategoria { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
    }

    public class EstadoTicketDto
    {
        public byte Status { get; set; }
        public string StatusDes { get; set; } = string.Empty;
    }

    public class PrioridadTicketDto
    {
        public byte IdPrioridad { get; set; }
        public string Prioridad { get; set; } = string.Empty;
    }

    public class TipoTicketDto
    {
        public byte IdTipoTicket { get; set; }
        public string TipoTicket { get; set; } = string.Empty;
    }

    public class DepartamentoDto
    {
        public byte IdDepto { get; set; }
        public string Departamento { get; set; } = string.Empty;
        public bool Tickets { get; set; }
    }

    public class PuestoDto
    {
        public byte IdPuesto { get; set; }
        public string Puesto { get; set; } = string.Empty;
        public byte IdDepto { get; set; }
        public string DepartamentoNombre { get; set; } = string.Empty;
    }

    public class PersonaDto
    {
        public int Persona { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Paterno { get; set; } = string.Empty;
        public string Materno { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string RFC { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TipoPersona { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
    }

    public class EmpleadoDto
    {
        public int IdPersona { get; set; }
        public string Login { get; set; }
        public bool Activo { get; set; }
        public PersonaDto PersonaInfo { get; set; }
        public List<PuestoDto> Puestos { get; set; } = new List<PuestoDto>();
    }

    public class TecnicoDto
    {
        public int IdPersona { get; set; }
        public string Login { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public byte? IdDepto { get; set; }
        public string Departamento { get; set; } = string.Empty;
    }

    public class RolDto
    {
        public byte IdRol { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string Descripcion { get; set; }
        public bool Activo { get; set; }
    }

    // DTOs para catálogos de activos
    public class TipoActivoDto
    {
        public byte IdTipoActivo { get; set; }
        public string TipoActivoNombre { get; set; } = string.Empty;
    }

    public class StatusActivoDto
    {
        public byte StatusId { get; set; }
        public string StatusNombre { get; set; } = string.Empty;
    }

    public class ProveedorDto
    {
        public byte IdProveedor { get; set; }
        public string ProveedorNombre { get; set; } = string.Empty;
    }

    public class ComponenteDto
    {
        public byte IdComponente { get; set; }
        public string ComponenteNombre { get; set; } = string.Empty;
        public int? ValorBit { get; set; }
    }

}
