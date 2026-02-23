using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using IndigoAsists.Repositorio;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssistMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para stdout (necesario para IIS/ASP.NET Core)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddEventSourceLogger();
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // Sesión de 1 hora
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Identity Core - DEBE configurarse ANTES de AddRepositorioServices
builder.Services
    .AddIdentityCore<IndigoAssits.Repositorio.Core.Entities.Usuario>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IndigoAssits.Repositorio.Core.Entities.Rol>()
    .AddEntityFrameworkStores<IndigoAsists.Repositorio.Db.IndigoDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager(); // Necesario para la autenticación de cookies

// Configuración de servicios del repositorio (incluye Identity)
builder.Services.AddRepositorioServices(builder.Configuration);

// Configuración de HttpClient para consumir la API
builder.Services.AddHttpContextAccessor(); // Necesario para acceder a la sesión desde el servicio

// Configurar HttpClient para servicios de API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5124";

// Servicio de autenticación
builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Servicio de usuarios
builder.Services.AddHttpClient<IUsuariosApiService, UsuariosApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Servicio de activos
builder.Services.AddHttpClient<IActivoApiService, ActivoApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Servicio de catálogos
builder.Services.AddHttpClient<ICatalogoApiService, CatalogoApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Servicio de tickets
builder.Services.AddHttpClient<ITicketApiService, TicketApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configuración de autenticación con cookies
// Identity.Application es el esquema que SignInManager usa por defecto
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    })
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.LoginPath = "/Usuario/Login";
        options.LogoutPath = "/Usuario/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Validar cadena de conexión antes de construir la app
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'DefaultConnection' en la configuración. " +
        "Asegúrese de que appsettings.json contenga la cadena de conexión.");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Middleware de sesión debe ir antes de Authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
