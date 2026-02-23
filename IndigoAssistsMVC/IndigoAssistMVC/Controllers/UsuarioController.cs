using Microsoft.AspNetCore.Mvc;
using IndigoAssistMVC.Models;
using IndigoAssistMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssistMVC.Services;

namespace IndigoAssistMVC.Controllers
{
    /// <summary>
    /// Controlador para gestión de usuarios con Identity
    /// </summary>
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly UserManager<IndigoAssits.Repositorio.Core.Entities.Usuario> _userManager;
        private readonly SignInManager<IndigoAssits.Repositorio.Core.Entities.Usuario> _signInManager;
        private readonly RoleManager<IndigoAssits.Repositorio.Core.Entities.Rol> _roleManager;
        private readonly IAuthApiService _authApiService;
        private readonly ICatalogoApiService _catalogoApiService;
        private readonly ITicketApiService _ticketApiService;
        private readonly IUsuariosApiService _usuariosApiService;

        public UsuarioController(
            ILogger<UsuarioController> logger, 
            UserManager<IndigoAssits.Repositorio.Core.Entities.Usuario> userManager, 
            SignInManager<IndigoAssits.Repositorio.Core.Entities.Usuario> signInManager,
            RoleManager<IndigoAssits.Repositorio.Core.Entities.Rol> roleManager,
            IAuthApiService authApiService,
            ICatalogoApiService catalogoApiService,
            ITicketApiService ticketApiService,
            IUsuariosApiService usuariosApiService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _authApiService = authApiService;
            _catalogoApiService = catalogoApiService;
            _ticketApiService = ticketApiService;
            _usuariosApiService = usuariosApiService;
        }

        /// <summary>
        /// Vista principal de gestión de usuarios con filtros
        /// </summary>
        public async Task<IActionResult> Index(UsuarioFiltroViewModel filtros, int pagina = 1, int tamanoPagina = 10)
        {
            var usuarios = await ObtenerUsuariosConFiltros(filtros);
            var usuariosPaginados = usuarios.Skip((pagina - 1) * tamanoPagina).Take(tamanoPagina).ToList();

            var viewModel = new UsuarioGestionViewModel
            {
                Usuarios = usuariosPaginados,
                Filtros = await PrepararFiltros(filtros),
                TotalUsuarios = usuarios.Count,
                PaginaActual = pagina,
                TotalPaginas = (int)Math.Ceiling((double)usuarios.Count / tamanoPagina),
                TamanoPagina = tamanoPagina
            };

            return View(viewModel);
        }

        /// <summary>
        /// Dashboard del usuario con estadísticas y filtros por rol
        /// </summary>
        public async Task<IActionResult> Dashboard(UsuarioFiltroViewModel filtros)
        {
            var usuarios = await ObtenerUsuariosConFiltros(filtros);
            var estadisticas = await ObtenerEstadisticasUsuarios();

            var viewModel = new UsuarioDashboardViewModel
            {
                Filtros = await PrepararFiltros(filtros),
                Usuarios = usuarios,
                EstadisticasGenerales = estadisticas,
                TotalUsuarios = usuarios.Count,
                UsuariosActivos = usuarios.Count(u => u.Activo),
                UsuariosInactivos = usuarios.Count(u => !u.Activo),
                UsuariosPorRol = usuarios.GroupBy(u => string.Join(", ", u.Roles))
                                       .ToDictionary(g => g.Key, g => g.Count())
            };

            return View(viewModel);
        }

        /// <summary>
        /// Detalles del usuario
        /// </summary>
        public async Task<IActionResult> Details(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            var departamentos = await ObtenerDepartamentosDisponibles();
            var departamento = departamentos.FirstOrDefault(d => d.IdDepartamento == usuario.IdDepartamento);

            var viewModel = new UsuarioViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "",
                NombreCompleto = usuario.NombreCompleto,
                DepartamentoNombre = departamento?.Nombre,
                Activo = usuario.Activo,
                FechaRegistro = usuario.FechaRegistro,
                UltimoAcceso = usuario.UltimoAcceso,
                Roles = roles.ToList(),
                EmailConfirmed = usuario.EmailConfirmed
            };

            return View(viewModel);
        }

        /// <summary>
        /// Vista para crear un nuevo usuario
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CrearUsuarioViewModel
            {
                DepartamentosDisponibles = await ObtenerDepartamentosDisponibles(),
                RolesDisponibles = await ObtenerRolesDisponibles()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Crear un nuevo usuario con contraseña genérica
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
                model.RolesDisponibles = await ObtenerRolesDisponibles();
                return View(model);
            }

            // Verificar si el usuario ya existe
            var usuarioExistente = await _userManager.FindByNameAsync(model.UserName);
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("UserName", "El nombre de usuario ya existe");
                model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
                model.RolesDisponibles = await ObtenerRolesDisponibles();
                return View(model);
            }

            // Crear nuevo usuario
            var usuario = new IndigoAssits.Repositorio.Core.Entities.Usuario
            {
                UserName = model.UserName,
                Email = model.Email,
                NombreCompleto = model.NombreCompleto,
                IdDepartamento = model.IdDepartamento,
                Activo = model.Activo,
                EmailConfirmed = true, // Confirmar email automáticamente
                FechaRegistro = DateTime.Now
            };

            // Contraseña genérica por defecto
            var passwordGenerica = "TempPassword123!";
            var resultado = await _userManager.CreateAsync(usuario, passwordGenerica);

            if (resultado.Succeeded)
            {
                // Asignar roles seleccionados
                if (model.RolesSeleccionados.Any())
                {
                    var rolesResult = await _userManager.AddToRolesAsync(usuario, model.RolesSeleccionados);
                    if (!rolesResult.Succeeded)
                    {
                        foreach (var error in rolesResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        await _userManager.DeleteAsync(usuario);
                        model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
                        model.RolesDisponibles = await ObtenerRolesDisponibles();
                        return View(model);
                    }
                }

                var requiereLegacy = model.RolesSeleccionados.Any(r =>
                    string.Equals(r, "Tecnico", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(r, "Supervisor", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(r, "Administrador", StringComparison.OrdinalIgnoreCase));

                if (requiereLegacy)
                {
                    if (string.IsNullOrWhiteSpace(model.LegacyLogin))
                    {
                        await _userManager.DeleteAsync(usuario);
                        ModelState.AddModelError("LegacyLogin", "El login legacy es requerido para roles de tickets.");
                        model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
                        model.RolesDisponibles = await ObtenerRolesDisponibles();
                        return View(model);
                    }

                    var legacyOk = await _usuariosApiService.EnsureLegacyUserAsync(
                        usuario.Email ?? string.Empty,
                        model.LegacyLogin,
                        model.IdDepartamento);

                    if (!legacyOk)
                    {
                        await _userManager.DeleteAsync(usuario);
                        ModelState.AddModelError("", "No se pudo crear el usuario legacy. Verifica que el email exista en vPersonas.");
                        model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
                        model.RolesDisponibles = await ObtenerRolesDisponibles();
                        return View(model);
                    }
                }

                TempData["SuccessMessage"] = $"Usuario '{model.UserName}' creado exitosamente con contraseña temporal: {passwordGenerica}";
                _logger.LogInformation("Usuario {Usuario} creado exitosamente", model.UserName);
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, agregarlos al ModelState
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
            model.RolesDisponibles = await ObtenerRolesDisponibles();
            return View(model);
        }

        /// <summary>
        /// Vista para editar un usuario
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            var viewModel = new EditarUsuarioViewModel
            {
                Id = usuario.Id,
                UserName = usuario.UserName ?? "",
                Email = usuario.Email ?? "",
                NombreCompleto = usuario.NombreCompleto,
                IdDepartamento = usuario.IdDepartamento,
                Activo = usuario.Activo,
                EmailConfirmed = usuario.EmailConfirmed,
                RolesSeleccionados = roles.ToList(),
                DepartamentosDisponibles = await ObtenerDepartamentosDisponibles(),
                RolesDisponibles = await ObtenerRolesDisponibles()
            };

            return View(viewModel);
        }

        /// <summary>
        /// Actualizar un usuario existente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarUsuarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
                model.RolesDisponibles = await ObtenerRolesDisponibles();
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(model.Id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Actualizar propiedades del usuario
            usuario.UserName = model.UserName;
            usuario.Email = model.Email;
            usuario.NombreCompleto = model.NombreCompleto;
            usuario.IdDepartamento = model.IdDepartamento;
            usuario.Activo = model.Activo;
            usuario.EmailConfirmed = model.EmailConfirmed;

            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                // Actualizar roles
                var rolesActuales = await _userManager.GetRolesAsync(usuario);
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
                
                if (model.RolesSeleccionados.Any())
                {
                    await _userManager.AddToRolesAsync(usuario, model.RolesSeleccionados);
                }

                TempData["SuccessMessage"] = $"Usuario '{model.UserName}' actualizado exitosamente";
                _logger.LogInformation("Usuario {Usuario} actualizado exitosamente", model.UserName);
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, agregarlos al ModelState
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            model.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
            model.RolesDisponibles = await ObtenerRolesDisponibles();
            return View(model);
        }

        /// <summary>
        /// Vista para cambiar contraseña
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CambiarPassword(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var viewModel = new CambiarPasswordViewModel
            {
                UsuarioId = usuario.Id,
                UserName = usuario.UserName ?? ""
            };

            return View(viewModel);
        }

        /// <summary>
        /// Cambiar contraseña de un usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _userManager.FindByIdAsync(model.UsuarioId);
            if (usuario == null)
            {
                return NotFound();
            }

            // Generar token de reset de contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultado = await _userManager.ResetPasswordAsync(usuario, token, model.NuevaPassword);

            if (resultado.Succeeded)
            {
                usuario.LockoutEnd = null;
                usuario.AccessFailedCount = 0;
                await _userManager.UpdateAsync(usuario);
                await _userManager.UpdateSecurityStampAsync(usuario);

                TempData["SuccessMessage"] = $"Contraseña cambiada exitosamente para el usuario '{model.UserName}'";
                _logger.LogInformation("Contraseña cambiada para usuario {Usuario}", model.UserName);
                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, agregarlos al ModelState
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Dar de baja (desactivar) un usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DarDeBaja(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Activo = false;
            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                TempData["SuccessMessage"] = $"Usuario '{usuario.UserName}' dado de baja exitosamente";
                _logger.LogInformation("Usuario {Usuario} dado de baja", usuario.UserName);
            }
            else
            {
                TempData["ErrorMessage"] = "Error al dar de baja el usuario";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Reactivar un usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reactivar(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            usuario.Activo = true;
            var resultado = await _userManager.UpdateAsync(usuario);

            if (resultado.Succeeded)
            {
                TempData["SuccessMessage"] = $"Usuario '{usuario.UserName}' reactivado exitosamente";
                _logger.LogInformation("Usuario {Usuario} reactivado", usuario.UserName);
            }
            else
            {
                TempData["ErrorMessage"] = "Error al reactivar el usuario";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Desbloquear usuario
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desbloquear(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            await _userManager.SetLockoutEndDateAsync(usuario, null);
            await _userManager.ResetAccessFailedCountAsync(usuario);

            TempData["SuccessMessage"] = $"Usuario '{usuario.UserName}' desbloqueado correctamente";
            _logger.LogInformation("Usuario {Usuario} desbloqueado", usuario.UserName);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Vista de login
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        /// <summary>
        /// Procesar login con Identity
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar usuario por nombre de usuario o email
            var user = await _userManager.FindByNameAsync(model.Usuario);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(model.Usuario);
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                return View(model);
            }

            // Verificar si el usuario está activo
            if (!user.Activo)
            {
                ModelState.AddModelError("", "El usuario está inactivo. Contacte al administrador.");
                return View(model);
            }

            // Verificar contraseña
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Recordarme, lockoutOnFailure: true);
            
            if (result.Succeeded)
            {
                // Actualizar último acceso
                user.UltimoAcceso = DateTime.Now;
                await _userManager.UpdateAsync(user);

                // Obtener token JWT de la API
                try
                {
                    // Verificar que la sesión esté disponible
                    if (HttpContext.Session == null)
                    {
                        _logger.LogWarning("La sesión no está disponible para guardar el token");
                    }
                    else
                    {
                        var tokenResponse = await _authApiService.LoginAsync(model.Usuario, model.Password);
                        if (tokenResponse != null)
                        {
                            // Guardar token en la sesión para uso en servicios de API
                            HttpContext.Session.SetString("ApiToken", tokenResponse.AccessToken);
                            HttpContext.Session.SetString("ApiTokenExpiresAt", tokenResponse.ExpiresAt.ToString("O"));
                            _logger.LogInformation("Token JWT obtenido y guardado en sesión para usuario: {Usuario}. Token: {TokenPreview}", 
                                model.Usuario, tokenResponse.AccessToken.Substring(0, Math.Min(20, tokenResponse.AccessToken.Length)) + "...");
                        }
                        else
                        {
                            _logger.LogWarning("No se pudo obtener token JWT de la API para usuario: {Usuario}. Verificar logs de la API.", model.Usuario);
                            await _signInManager.SignOutAsync();
                            ModelState.AddModelError(string.Empty, "No se pudo iniciar sesión en la API. Verifica que la API esté ejecutándose y que las credenciales sean válidas.");
                            return View(model);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // No fallar el login si hay error al obtener el token, solo loguear
                    _logger.LogError(ex, "Error al obtener token JWT de la API para usuario: {Usuario}. El login no continuará. Error: {Error}", 
                        model.Usuario, ex.Message);
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "No se pudo autenticar contra la API. Revisa que la API esté en línea y el JWT configurado.");
                    return View(model);
                }

                // Establecer variables de sesión
                HttpContext.Session.SetString("Login", user.UserName ?? "");
                HttpContext.Session.SetString("NombreCompleto", user.NombreCompleto);

                try
                {
                    var userContext = await _ticketApiService.GetUsuarioContextoAsync();
                    if (userContext != null)
                    {
                        HttpContext.Session.Remove("LegacyTicketsMissing");
                        HttpContext.Session.SetInt32("IdPersona", userContext.IdPersona);
                        if (!string.IsNullOrWhiteSpace(userContext.Login))
                        {
                            HttpContext.Session.SetString("Login", userContext.Login);
                        }
                        if (!string.IsNullOrWhiteSpace(userContext.NombreCompleto))
                        {
                            HttpContext.Session.SetString("NombreCompleto", userContext.NombreCompleto);
                        }
                        if (userContext.IdDepto.HasValue)
                        {
                            HttpContext.Session.SetInt32("IdDepto", userContext.IdDepto.Value);
                        }
                        if (!string.IsNullOrWhiteSpace(userContext.Departamento))
                        {
                            HttpContext.Session.SetString("DepartamentoNombre", userContext.Departamento);
                        }
                    }
                    else
                    {
                        HttpContext.Session.Remove("IdPersona");
                        HttpContext.Session.Remove("IdDepto");
                        HttpContext.Session.Remove("DepartamentoNombre");
                        HttpContext.Session.SetString("LegacyTicketsMissing", "true");
                        _logger.LogWarning("No se encontró contexto legacy para el usuario {Usuario}.", model.Usuario);
                    }
                }
                catch (Exception ex)
                {
                    HttpContext.Session.SetString("LegacyTicketsMissing", "true");
                    _logger.LogError(ex, "Error al obtener el contexto legacy para usuario: {Usuario}", model.Usuario);
                }

                _logger.LogInformation("Usuario {Usuario} autenticado exitosamente", model.Usuario);

                // Redirigir según el rol
                var roles = await _userManager.GetRolesAsync(user);
                var returnUrl = await GetRedirectUrlByRole(roles.FirstOrDefault());

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return Redirect(returnUrl);
            }
            else
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "El usuario está bloqueado. Contacte al administrador para desbloqueo.");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos");
                }
                return View(model);
            }
        }

        /// <summary>
        /// Cerrar sesión
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Limpiar la sesión (incluye el token JWT)
            HttpContext.Session.Clear();
            
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión");
            return RedirectToAction("Login");
        }

        #region Métodos Auxiliares

        /// <summary>
        /// Obtener usuarios con filtros aplicados
        /// </summary>
        private async Task<List<UsuarioViewModel>> ObtenerUsuariosConFiltros(UsuarioFiltroViewModel filtros)
        {
            var query = _userManager.Users.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtros.Buscar))
            {
                query = query.Where(u => u.UserName.Contains(filtros.Buscar) || 
                                       u.NombreCompleto.Contains(filtros.Buscar) ||
                                       u.Email.Contains(filtros.Buscar));
            }

            if (filtros.IdDepartamentoFiltro.HasValue)
            {
                query = query.Where(u => u.IdDepartamento == filtros.IdDepartamentoFiltro.Value);
            }

            if (filtros.ActivoFiltro.HasValue)
            {
                query = query.Where(u => u.Activo == filtros.ActivoFiltro.Value);
            }

            if (filtros.EmailConfirmedFiltro.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == filtros.EmailConfirmedFiltro.Value);
            }

            var usuarios = await query.ToListAsync();
            var usuariosViewModel = new List<UsuarioViewModel>();
            var departamentos = await ObtenerDepartamentosDisponibles();
            var departamentosMap = departamentos
                .GroupBy(d => d.IdDepartamento)
                .ToDictionary(g => g.Key, g => g.First().Nombre);

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                departamentosMap.TryGetValue(usuario.IdDepartamento ?? 0, out var departamentoNombre);

                // Aplicar filtro por rol si está especificado
                if (!string.IsNullOrEmpty(filtros.RolFiltro) && !roles.Contains(filtros.RolFiltro))
                {
                    continue;
                }

                usuariosViewModel.Add(new UsuarioViewModel
                {
                    Id = usuario.Id,
                    UserName = usuario.UserName ?? "",
                    Email = usuario.Email ?? "",
                    NombreCompleto = usuario.NombreCompleto,
                    DepartamentoNombre = departamentoNombre,
                    Activo = usuario.Activo,
                    FechaRegistro = usuario.FechaRegistro,
                    UltimoAcceso = usuario.UltimoAcceso,
                    Roles = roles.ToList(),
                    EmailConfirmed = usuario.EmailConfirmed,
                    IsLockedOut = await _userManager.IsLockedOutAsync(usuario),
                    LockoutEnd = usuario.LockoutEnd
                });
            }

            return usuariosViewModel;
        }

        /// <summary>
        /// Preparar filtros con datos disponibles
        /// </summary>
        private async Task<UsuarioFiltroViewModel> PrepararFiltros(UsuarioFiltroViewModel filtros)
        {
            filtros.RolesDisponibles = await ObtenerRolesDisponibles();
            filtros.DepartamentosDisponibles = await ObtenerDepartamentosDisponibles();
            return filtros;
        }

        /// <summary>
        /// Obtener departamentos disponibles
        /// </summary>
        private async Task<List<DepartamentoViewModel>> ObtenerDepartamentosDisponibles()
        {
            var departamentos = await _catalogoApiService.GetDepartamentosAsync();
            return departamentos.Select(d => new DepartamentoViewModel
            {
                IdDepartamento = d.IdDepto,
                Nombre = d.Departamento
            }).ToList();
        }

        /// <summary>
        /// Obtener roles disponibles
        /// </summary>
        private async Task<List<RolViewModel>> ObtenerRolesDisponibles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new RolViewModel
            {
                Id = r.Id,
                Name = r.Name ?? "",
                Descripcion = $"Rol de {r.Name}"
            }).ToList();
        }

        /// <summary>
        /// Obtener estadísticas de usuarios
        /// </summary>
        private async Task<Dictionary<string, int>> ObtenerEstadisticasUsuarios()
        {
            var totalUsuarios = await _userManager.Users.CountAsync();
            var usuariosActivos = await _userManager.Users.CountAsync(u => u.Activo);
            var usuariosInactivos = totalUsuarios - usuariosActivos;
            var usuariosConEmailConfirmado = await _userManager.Users.CountAsync(u => u.EmailConfirmed);

            return new Dictionary<string, int>
            {
                { "Total Usuarios", totalUsuarios },
                { "Usuarios Activos", usuariosActivos },
                { "Usuarios Inactivos", usuariosInactivos },
                { "Emails Confirmados", usuariosConEmailConfirmado }
            };
        }

        /// <summary>
        /// Obtener URL de redirección según el rol del usuario
        /// </summary>
        private Task<string> GetRedirectUrlByRole(string? role)
        {
            return Task.FromResult(role switch
            {
                "Administrador" => "/Home/Index", // Administradores van al Home (acceso completo)
                "Supervisor" => "/Ticket/Index",  // Supervisores van al dashboard de tickets
                "Tecnico" => "/Ticket/Index",     // Técnicos van al dashboard de tickets
                _ => "/Home/Index"                // Por defecto al Home
            });
        }

        #endregion
    }
}
