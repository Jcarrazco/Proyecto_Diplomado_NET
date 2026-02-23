using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndigoAssits.Core.Dtos
{
    public class ApiResponseDto<T>
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T Datos { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ApiResponseDto
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<string> Errores { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class ApiResponsePaginadoDto<T>
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<T> Datos { get; set; } = new List<T>();
        public PaginacionDto Paginacion { get; set; } = new PaginacionDto();
        public List<string> Errores { get; set; } = new List<string>();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class PaginacionDto
    {
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public int TamañoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public bool TienePaginaAnterior { get; set; }
        public bool TienePaginaSiguiente { get; set; }
    }

    public class FiltroFechaDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    public class FiltroTextoDto
    {
        public string? BusquedaTexto { get; set; }
        public bool BusquedaExacta { get; set; } = false;
    }

    public class FiltroEstadoDto
    {
        public bool? Activo { get; set; }
    }

    public class FiltroDepartamentoDto
    {
        public byte? IdDepartamento { get; set; }
    }

    public class FiltroBasicoDto
    {
        public string? BusquedaTexto { get; set; }
        public bool? Activo { get; set; }
        public byte? IdDepartamento { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
    }

    public class ArchivoAdjuntoDto
    {
        public int IdArchivo { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public string TipoMime { get; set; } = string.Empty;
        public long TamañoBytes { get; set; }
        public string RutaArchivo { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
        public int? IdTicket { get; set; }
        public string? UsuarioSubida { get; set; }
    }

    public class SubirArchivoDto
    {
        [Required(ErrorMessage = "El archivo es requerido")]
        public byte[] ContenidoArchivo { get; set; } = Array.Empty<byte>();

        [Required(ErrorMessage = "El nombre del archivo es requerido")]
        [StringLength(255, ErrorMessage = "El nombre del archivo no puede exceder 255 caracteres")]
        public string NombreArchivo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo MIME es requerido")]
        public string TipoMime { get; set; } = string.Empty;

        public int? IdTicket { get; set; }
    }

    public class ComentarioTicketDto
    {
        public int IdComentario { get; set; }
        public int IdTicket { get; set; }
        public string UsuarioComentario { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public DateTime FechaComentario { get; set; }
    }

    public class CrearComentarioDto
    {
        [Required(ErrorMessage = "El ID del ticket es requerido")]
        public int IdTicket { get; set; }

        [Required(ErrorMessage = "El comentario es requerido")]
        [StringLength(2000, ErrorMessage = "El comentario no puede exceder 2000 caracteres")]
        public string Comentario { get; set; } = string.Empty;
    }

    public class NotificacionDto
    {
        public int IdNotificacion { get; set; }
        public string UsuarioDestino { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string TipoNotificacion { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaLectura { get; set; }
        public int? IdTicket { get; set; }
    }

    public class CrearNotificacionDto
    {
        [Required(ErrorMessage = "El usuario destino es requerido")]
        public string UsuarioDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(100, ErrorMessage = "El título no puede exceder 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es requerido")]
        [StringLength(500, ErrorMessage = "El mensaje no puede exceder 500 caracteres")]
        public string Mensaje { get; set; } = string.Empty;

        public int? IdTicket { get; set; }
    }
}
