using IndigoAsists.Repositorio;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssitsReglasDeNegocio;
using IndigoAssits.API.Data;
using IndigoAssits.API.Infrastructure.Legacy;
using IndigoAssits.API.Infrastructure.Swagger;
using IndigoAssits.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers con configuración JSON para aceptar camelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Identity Core - DEBE configurarse ANTES de AddRepositorioServices
builder.Services
    .AddIdentityCore<Usuario>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<Rol>()
    .AddEntityFrameworkStores<IndigoAsists.Repositorio.Db.IndigoDbContext>()
    .AddDefaultTokenProviders();

// Repositorio (EF + repos) y capa de negocio
builder.Services.AddRepositorioServices(builder.Configuration);
builder.Services.AddReglasDeNegocio();

// HttpContext y conexión legacy por sucursal
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILegacyConnectionResolver, LegacyConnectionResolver>();
builder.Services.AddScoped<ILegacyDbConnectionFactory, LegacyDbConnectionFactory>();
builder.Services.AddScoped<ILegacyUserContextService, LegacyUserContextService>();

builder.Services.AddDbContext<ActivosDbContext>((sp, options) =>
{
    var resolver = sp.GetRequiredService<ILegacyConnectionResolver>();
    var connectionString = resolver.Resolve();
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
    });
});


// Servicios legacy (SPs) para tickets y activos
builder.Services.AddScoped<IndigoAssitsReglasDeNegocio.Interfaces.ITicketService, LegacyTicketService>();
builder.Services.AddScoped<IndigoAssitsReglasDeNegocio.Interfaces.IActivoService, ActivosEfService>();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"] ?? "changeme"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = signingKey,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger + Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IndigoAssist API", Version = "v1" });
    c.OperationFilter<SucursalHeaderOperationFilter>();
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingrese el token JWT como: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new string[] { } }
    });
});

var app = builder.Build();

// Pipeline
// Middleware de manejo de errores global
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Error no controlado en la API");
        
        var errorResponse = new
        {
            error = "Error interno del servidor",
            message = exception?.Message ?? "Error desconocido",
            path = exceptionHandlerPathFeature?.Path
        };
        
        await context.Response.WriteAsJsonAsync(errorResponse);
    });
});

// Habilitar Swagger en todos los entornos (incluyendo producción)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IndigoAssist API v1");
    c.RoutePrefix = "swagger";
});

// HTTPS Redirection condicional (algunos hosts como Somee pueden no tener HTTPS configurado)
var enableHttpsRedirection = builder.Configuration.GetValue<bool>("EnableHttpsRedirection", false);
if (enableHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
