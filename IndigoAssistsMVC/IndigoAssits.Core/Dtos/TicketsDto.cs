using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoAssits.Core.Dtos
{
    public class TicketsDtoIn
    {
        [Required(ErrorMessage = "El usuario solicitante es requerido")]
        public int UsuarioSolicitante { get; set; }

        [Required(ErrorMessage = "La subcategoría es requerida")]
        public byte IdSubCategoria { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(50, ErrorMessage = "El título no puede exceder 50 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        public byte? IdTipoTicket { get; set; }

        public byte? Prioridad { get; set; }
    }
}
