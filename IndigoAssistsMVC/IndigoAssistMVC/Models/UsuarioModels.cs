using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IndigoAssistMVC.Models
{
    /// <summary>
    /// Modelo para usuarios del sistema con Identity
    /// </summary>
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

        // Propiedades de navegación
        [ForeignKey("IdDepartamento")]
        public virtual mDepartamentos? Departamento { get; set; }

        // Propiedades calculadas para estadísticas
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

    /// <summary>
    /// Modelo para roles de usuario
    /// </summary>
    [Table("Roles")]
    public class AspNetRol
    {
        [Key]
        [Display(Name = "ID Rol")]
        public byte IdRol { get; set; }

        [Display(Name = "Nombre del Rol")]
        [Required, StringLength(50)]
        public string NombreRol { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }

    /// <summary>
    /// ViewModel para el login de usuarios
    /// </summary>
    public class LoginViewModel
    {
        [Display(Name = "Usuario")]
        [Required(ErrorMessage = "El usuario es requerido")]
        [StringLength(50, ErrorMessage = "El usuario no puede exceder 50 caracteres")]
        public string Usuario { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
