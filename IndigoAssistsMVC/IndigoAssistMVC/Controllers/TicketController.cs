using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IndigoAssistMVC.Models;
using IndigoAssistMVC.Services;
using IndigoAssits.Core.Dtos;

namespace IndigoAssistMVC.Controllers
{
    /// <summary>
    /// Controlador para el módulo de Tickets
    /// Maneja la autenticación, consultas y gestión de tickets
    /// </summary>
    [Authorize(Roles = "Administrador,Supervisor,Tecnico")]
    public class TicketController : Controller
    {
        private readonly ILogger<TicketController> _logger;
        private readonly ITicketApiService _ticketApiService;
        private readonly ICatalogoApiService _catalogoApiService;

        public TicketController(
            ILogger<TicketController> logger,
            ITicketApiService ticketApiService,
            ICatalogoApiService catalogoApiService)
        {
            _logger = logger;
            _ticketApiService = ticketApiService;
            _catalogoApiService = catalogoApiService;
        }

        /// <summary>
        /// Página principal del módulo de tickets
        /// Muestra el dashboard con estadísticas y tickets abiertos
        /// </summary>
        public async Task<IActionResult> Index()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            var isAdmin = User.IsInRole("Administrador");
            var legacyMissing = string.Equals(HttpContext.Session.GetString("LegacyTicketsMissing"), "true", StringComparison.OrdinalIgnoreCase);

            if (legacyMissing)
            {
                if (isAdmin)
                {
                    var tickets = await _ticketApiService.GetTicketsAsync();
                    ViewBag.LegacyMissing = true;
                    ViewBag.LegacyAdmin = true;
                    ViewBag.Titulo = "Todos los tickets";
                    ViewBag.TipoConsulta = "todos";
                    ViewBag.EstadoActual = "Todos";
                    ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync(includeAll: true);
                    return View("TicketsGenerico", tickets);
                }

                ViewBag.LegacyMissing = true;
                return View(new TicketDashboardViewModel());
            }

            // Verificar si el usuario está autenticado
            var idPersona = HttpContext.Session.GetInt32("IdPersona");
            if (idPersona == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var dashboard = await ObtenerDashboardTickets(idPersona.Value);
                return View(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el dashboard de tickets para IdPersona: {IdPersona}", idPersona);
                TempData["Error"] = "Error al cargar el dashboard de tickets";
                return View(new TicketDashboardViewModel());
            }
        }


        /// <summary>
        /// Obtiene tickets abiertos por departamento del usuario
        /// </summary>
        public async Task<IActionResult> TicketsAbiertosDepto()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            if (TryHandleLegacyMissing(out var legacyResult))
            {
                return legacyResult;
            }

            var idPersona = HttpContext.Session.GetInt32("IdPersona");
            if (idPersona == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var tickets = await ObtenerTicketsAbiertosPorDepartamento(idPersona.Value);
                ViewBag.Titulo = "Tickets Abiertos por Departamento";
                ViewBag.TipoConsulta = "departamento";
                ViewBag.EstadoActual = "Abierto";
                ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync();
                ViewBag.Departamentos = await ObtenerDepartamentosParaFiltroAsync();
                return View("TicketsGenerico", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets abiertos por departamento");
                TempData["Error"] = "Error al cargar tickets abiertos";
                return View("TicketsGenerico", new List<TicketVista>());
            }
        }

        /// <summary>
        /// Obtiene tickets en proceso por departamento del usuario
        /// </summary>
        public async Task<IActionResult> TicketsEnProcesoDepto()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            if (TryHandleLegacyMissing(out var legacyResult))
            {
                return legacyResult;
            }

            var idPersona = HttpContext.Session.GetInt32("IdPersona");
            if (idPersona == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var tickets = await ObtenerTicketsEnProcesoPorDepartamento(idPersona.Value);
                ViewBag.Titulo = "Tickets En Proceso por Departamento";
                ViewBag.TipoConsulta = "departamento";
                ViewBag.EstadoActual = "En Proceso";
                ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync();
                ViewBag.Departamentos = await ObtenerDepartamentosParaFiltroAsync();
                return View("TicketsGenerico", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets en proceso por departamento");
                TempData["Error"] = "Error al cargar tickets en proceso";
                return View("TicketsGenerico", new List<TicketVista>());
            }
        }

        /// <summary>
        /// Obtiene tickets en proceso asignados al técnico actual
        /// </summary>
        public async Task<IActionResult> TicketsAsignados()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            if (TryHandleLegacyMissing(out var legacyResult))
            {
                return legacyResult;
            }

            var idPersona = HttpContext.Session.GetInt32("IdPersona");
            if (idPersona == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var tickets = await ObtenerTicketsAsignadosAlTecnico(idPersona.Value);
                ViewBag.Titulo = "Tickets Asignados a Mí";
                ViewBag.TipoConsulta = "asignados";
                ViewBag.EstadoActual = "En Proceso";
                ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync();
                ViewBag.Departamentos = await ObtenerDepartamentosParaFiltroAsync();
                return View("TicketsGenerico", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets asignados al técnico");
                TempData["Error"] = "Error al cargar tickets asignados";
                return View("TicketsGenerico", new List<TicketVista>());
            }
        }

        /// <summary>
        /// Obtiene tickets cerrados por departamento
        /// </summary>
        public async Task<IActionResult> TicketsCerradosDepto()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            if (TryHandleLegacyMissing(out var legacyResult))
            {
                return legacyResult;
            }

            var idPersona = HttpContext.Session.GetInt32("IdPersona");
            if (idPersona == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var tickets = await ObtenerTicketsCerradosPorDepartamento(idPersona.Value);
                ViewBag.Titulo = "Tickets Cerrados por Departamento";
                ViewBag.TipoConsulta = "departamento";
                ViewBag.EstadoActual = "Cerrado";
                ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync();
                ViewBag.Departamentos = await ObtenerDepartamentosParaFiltroAsync();
                return View("TicketsGenerico", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets cerrados por departamento");
                TempData["Error"] = "Error al cargar tickets cerrados";
                return View("TicketsGenerico", new List<TicketVista>());
            }
        }

        /// <summary>
        /// Obtiene tickets cerrados por el usuario actual
        /// </summary>
        public async Task<IActionResult> TicketsCerradosUsuario()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            if (TryHandleLegacyMissing(out var legacyResult))
            {
                return legacyResult;
            }

            var idPersona = HttpContext.Session.GetInt32("IdPersona");
            if (idPersona == null)
            {
                return RedirectToAction("Login", "Usuario");
            }

            try
            {
                var tickets = await ObtenerTicketsCerradosPorUsuario(idPersona.Value);
                ViewBag.Titulo = "Mis Tickets Cerrados";
                ViewBag.TipoConsulta = "usuario";
                ViewBag.EstadoActual = "Cerrado";
                ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync();
                ViewBag.Departamentos = await ObtenerDepartamentosParaFiltroAsync();
                return View("TicketsGenerico", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tickets cerrados por usuario");
                TempData["Error"] = "Error al cargar tickets cerrados";
                return View("TicketsGenerico", new List<TicketVista>());
            }
        }

        /// <summary>
        /// Obtiene todos los tickets (solo Administrador)
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> TicketsTodos()
        {
            if (TryHandleExpiredToken(out var tokenResult))
            {
                return tokenResult;
            }

            try
            {
                var filtros = EnsurePaging(new TicketFiltroDto());
                var tickets = await _ticketApiService.GetTicketsAsync(filtros);
                ViewBag.Titulo = "Todos los tickets";
                ViewBag.TipoConsulta = "todos";
                ViewBag.EstadoActual = "Todos";
                ViewBag.Tecnicos = await ObtenerTecnicosParaAsignacionAsync(includeAll: true);
                ViewBag.Departamentos = await ObtenerDepartamentosParaFiltroAsync(includeAll: true);
                return View("TicketsGenerico", tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los tickets");
                TempData["Error"] = "Error al cargar todos los tickets";
                return View("TicketsGenerico", new List<TicketVista>());
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Obtiene el dashboard completo de tickets para el usuario
        /// </summary>
        private async Task<TicketDashboardViewModel> ObtenerDashboardTickets(int idPersona)
        {
            var dashboard = new TicketDashboardViewModel();

            var dashboardDto = await _ticketApiService.GetDashboardAsync("depto");
            if (dashboardDto == null)
            {
                return dashboard;
            }

            dashboard.Usuario = MapUsuarioContexto(dashboardDto.Contexto);
            dashboard.TicketsAbiertosDepto = dashboardDto.Listas.AbiertosDepto.Select(MapDtoToTicketVista).ToList();
            dashboard.TicketsEnProcesoDepto = dashboardDto.Listas.EnProcesoDepto.Select(MapDtoToTicketVista).ToList();
            dashboard.TicketsEnProcesoAsignados = dashboardDto.Listas.Asignados.Select(MapDtoToTicketVista).ToList();
            dashboard.TicketsCerradosDepto = new List<TicketVista>();
            dashboard.TicketsCerradosUsuario = new List<TicketVista>();

            dashboard.Estadisticas = CalcularEstadisticas(dashboard);

            return dashboard;
        }

        /// <summary>
        /// Obtiene tickets abiertos (Status = 1) por departamento del usuario
        /// </summary>
        private async Task<List<TicketVista>> ObtenerTicketsAbiertosPorDepartamento(int idPersona)
        {
            var usuarioDepto = ObtenerDepartamentoDesdeSesion();
            if (usuarioDepto == 0)
            {
                return new List<TicketVista>();
            }

            var filtros = new TicketFiltroDto
            {
                IdDepartamento = usuarioDepto
            };

            return await _ticketApiService.GetTicketsAbiertosAsync(filtros);
        }

        /// <summary>
        /// Obtiene tickets en proceso (Status = 2) por departamento del usuario
        /// </summary>
        private async Task<List<TicketVista>> ObtenerTicketsEnProcesoPorDepartamento(int idPersona)
        {
            var usuarioDepto = ObtenerDepartamentoDesdeSesion();
            if (usuarioDepto == 0)
            {
                return new List<TicketVista>();
            }

            var filtros = new TicketFiltroDto
            {
                IdDepartamento = usuarioDepto,
                Status = "en_proceso"
            };

            return await _ticketApiService.GetTicketsAsync(EnsurePaging(filtros));
        }

        /// <summary>
        /// Obtiene tickets en proceso asignados al técnico actual
        /// </summary>
        private async Task<List<TicketVista>> ObtenerTicketsAsignadosAlTecnico(int idPersona)
        {
            var filtros = new TicketFiltroDto
            {
                IdTecnico = idPersona,
                Status = "en_proceso"
            };

            return await _ticketApiService.GetTicketsAsync(EnsurePaging(filtros));
        }

        /// <summary>
        /// Obtiene tickets cerrados (Status = 3) por departamento del usuario
        /// </summary>
        private async Task<List<TicketVista>> ObtenerTicketsCerradosPorDepartamento(int idPersona)
        {
            var usuarioDepto = ObtenerDepartamentoDesdeSesion();
            if (usuarioDepto == 0)
            {
                return new List<TicketVista>();
            }

            var filtros = new TicketFiltroDto
            {
                IdDepartamento = usuarioDepto,
                Status = "cerrado"
            };

            var tickets = await _ticketApiService.GetTicketsAsync(EnsurePaging(filtros));
            return tickets;
        }

        /// <summary>
        /// Obtiene tickets cerrados por el usuario actual
        /// </summary>
        private async Task<List<TicketVista>> ObtenerTicketsCerradosPorUsuario(int idPersona)
        {
            var filtros = new TicketFiltroDto
            {
                UsuarioSolicitante = idPersona,
                Status = "cerrado"
            };

            var tickets = await _ticketApiService.GetTicketsAsync(EnsurePaging(filtros));
            return tickets;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarTecnico(int idTicket, int tecnicoId, string? returnUrl = null)
        {
            if (!User.IsInRole("Administrador") && !User.IsInRole("Supervisor"))
            {
                return Forbid();
            }

            if (idTicket <= 0 || tecnicoId <= 0)
            {
                TempData["Error"] = "Datos de asignación inválidos.";
                return RedirectToAction("TicketsAbiertosDepto");
            }

            var tecnicos = await ObtenerTecnicosParaAsignacionAsync();
            if (!User.IsInRole("Administrador") && tecnicos.All(t => t.IdPersona != tecnicoId))
            {
                TempData["Error"] = "El técnico seleccionado no pertenece a tu departamento.";
                return RedirectToAction("TicketsAbiertosDepto");
            }

            var ok = await _ticketApiService.AsignarTecnicoAsync(idTicket, tecnicoId);
            if (!ok)
            {
                TempData["Error"] = "No se pudo asignar el técnico.";
            }
            else
            {
                TempData["Info"] = "Técnico asignado correctamente.";
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("TicketsAbiertosDepto");
        }

        private async Task<List<TecnicoDto>> ObtenerTecnicosParaAsignacionAsync(bool includeAll = false)
        {
            if (includeAll || User.IsInRole("Administrador"))
            {
                return await _ticketApiService.GetTecnicosAsync();
            }

            var depto = ObtenerDepartamentoDesdeSesion();
            if (depto == 0)
            {
                return new List<TecnicoDto>();
            }

            return await _ticketApiService.GetTecnicosAsync(depto);
        }

        private static TicketFiltroDto EnsurePaging(TicketFiltroDto filtros)
        {
            if (filtros.Pagina <= 0)
            {
                filtros.Pagina = 1;
            }

            if (filtros.TamañoPagina <= 0)
            {
                filtros.TamañoPagina = 200;
            }

            if (filtros.TamañoPagina < 50)
            {
                filtros.TamañoPagina = 200;
            }

            return filtros;
        }

        private byte ObtenerDepartamentoDesdeSesion()
        {
            var depto = HttpContext.Session.GetInt32("IdDepto");
            if (depto.HasValue && depto.Value > 0 && depto.Value <= byte.MaxValue)
            {
                return (byte)depto.Value;
            }

            return 0;
        }

        private bool TryHandleExpiredToken(out IActionResult result)
        {
            result = null;
            var token = HttpContext.Session.GetString("ApiToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Error"] = "No hay un token de API activo. Inicia sesión nuevamente.";
                result = RedirectToAction("Login", "Usuario");
                return true;
            }

            var expiresAtRaw = HttpContext.Session.GetString("ApiTokenExpiresAt");
            if (string.IsNullOrWhiteSpace(expiresAtRaw))
            {
                return false;
            }

            if (DateTime.TryParse(expiresAtRaw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var expiresAt))
            {
                if (expiresAt <= DateTime.UtcNow)
                {
                    HttpContext.Session.Remove("ApiToken");
                    HttpContext.Session.Remove("ApiTokenExpiresAt");
                    TempData["Error"] = "Tu sesión de API expiró. Inicia sesión nuevamente.";
                    result = RedirectToAction("Login", "Usuario");
                    return true;
                }
            }

            return false;
        }

        private bool TryHandleLegacyMissing(out IActionResult result)
        {
            result = null;
            var legacyMissing = string.Equals(HttpContext.Session.GetString("LegacyTicketsMissing"), "true", StringComparison.OrdinalIgnoreCase);
            if (!legacyMissing)
            {
                return false;
            }

            if (User.IsInRole("Administrador"))
            {
                result = RedirectToAction("TicketsTodos");
                return true;
            }

            ViewBag.LegacyMissing = true;
            ViewBag.Titulo = "Tickets";
            ViewBag.TipoConsulta = "departamento";
            ViewBag.EstadoActual = "Abierto";
            ViewBag.Tecnicos = new List<TecnicoDto>();
            ViewBag.Departamentos = new List<mDepartamentos>();
            TempData["Error"] = "Este usuario no pertenece al sistema legacy de tickets.";
            result = View("TicketsGenerico", new List<TicketVista>());
            return true;
        }

        private async Task<List<mDepartamentos>> ObtenerDepartamentosParaFiltroAsync(bool includeAll = false)
        {
            if (includeAll || User.IsInRole("Administrador"))
            {
                return await _catalogoApiService.GetDepartamentosAsync();
            }

            var depto = ObtenerDepartamentoDesdeSesion();
            if (depto == 0)
            {
                return new List<mDepartamentos>();
            }

            var departamentos = await _catalogoApiService.GetDepartamentosAsync();
            return departamentos.Where(d => d.IdDepto == depto).ToList();
        }

        private static TicketVista MapDtoToTicketVista(TicketResponseDto dto)
        {
            return new TicketVista
            {
                IdTicket = dto.IdTicket,
                IdSolicitante = dto.UsuarioSolicitante,
                Solicitante = dto.SolicitanteNombre,
                IdTecnico = dto.IdTecnico == 0 ? null : dto.IdTecnico,
                Tecnico = dto.TecnicoNombre ?? string.Empty,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                Status = dto.Status,
                StatusDes = dto.StatusDescripcion,
                IdTipoTicket = dto.IdTipoTicket,
                IdPrioridad = dto.Prioridad,
                FeAlta = dto.FeAlta,
                FeAsignacion = dto.FeAsignacion == default ? null : dto.FeAsignacion,
                FeCompromiso = dto.FeCompromiso == default ? null : dto.FeCompromiso,
                FeCierre = dto.FeCierre == default ? null : dto.FeCierre,
                IdSubCategoria = dto.IdSubCategoria,
                SubCategoria = dto.SubCategoriaNombre,
                IdCategoria = dto.IdCategoria,
                Categoria = dto.CategoriaNombre,
                IdDepto = dto.IdDepartamento,
                Departamento = dto.DepartamentoNombre
            };
        }

        private static mPerEmp MapUsuarioContexto(UserContextDto context)
        {
            var persona = new mPersonas
            {
                Nombre = context.Nombre,
                Paterno = context.Paterno,
                Materno = context.Materno,
                Usuario = context.Login
            };

            return new mPerEmp
            {
                IdPersona = context.IdPersona,
                PersonaInfo = persona
            };
        }

        /// <summary>
        /// Calcula las estadísticas del dashboard
        /// </summary>
        private TicketEstadisticasViewModel CalcularEstadisticas(TicketDashboardViewModel dashboard)
        {
            var estadisticas = new TicketEstadisticasViewModel
            {
                TotalAbiertos = dashboard.TicketsAbiertosDepto.Count,
                TotalEnProceso = dashboard.TicketsEnProcesoDepto.Count,
                TotalAsignados = dashboard.TicketsEnProcesoAsignados.Count,
                TotalCerrados = dashboard.TicketsCerradosDepto.Count + dashboard.TicketsCerradosUsuario.Count
            };

            // Estadísticas por departamento
            var todosLosTickets = dashboard.TicketsAbiertosDepto
                .Concat(dashboard.TicketsEnProcesoDepto)
                .Concat(dashboard.TicketsCerradosDepto)
                .ToList();

            estadisticas.PorDepartamento = todosLosTickets
                .GroupBy(t => t.Departamento)
                .ToDictionary(g => g.Key, g => g.Count());

            // Estadísticas por prioridad
            estadisticas.PorPrioridad = todosLosTickets
                .Where(t => t.IdPrioridad.HasValue)
                .GroupBy(t => t.IdPrioridad.Value)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            return estadisticas;
        }

        #endregion
    }
}
