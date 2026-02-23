using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IndigoAssits.Repositorio.Core.Entities
{
    public class Usuario : IdentityUser
    {
        [Display(Name = "Nombre Completo")]
        [Required, StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Departamento")]
        public byte? IdDepartamento { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Último Acceso")]
        public DateTime? UltimoAcceso { get; set; }

        [ForeignKey("IdDepartamento")]
        public virtual mDepartamentos? Departamento { get; set; }

        [NotMapped]
        [Display(Name = "Tickets Solucionados")]
        public int TicketsSolucionados { get; set; }

        [NotMapped]
        [Display(Name = "Tickets en Proceso")]
        public int TicketsEnProceso { get; set; }

        [NotMapped]
        [Display(Name = "Tickets Pendientes")]
        public int TicketsPendientes { get; set; }

        [NotMapped]
        [Display(Name = "Tiempo Promedio Resolución")]
        public TimeSpan? TiempoPromedioResolucion { get; set; }
    }

    public class Rol : IdentityRole
    {
        [Display(Name = "Descripción")]
        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
