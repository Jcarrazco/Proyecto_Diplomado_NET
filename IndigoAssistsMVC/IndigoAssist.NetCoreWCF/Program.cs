using CoreWCF.Configuration;
using CoreWCF.Description;
using IndigoAsists.Repositorio;
using IndigoAssist.NetCoreWCF.Services;
using IndigoAssits.Repositorio.Core.Entities;
using IndigoAssitsReglasDeNegocio;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

builder.Services.AddRepositorioServices(builder.Configuration);
builder.Services.AddReglasDeNegocio();

builder.Services.AddScoped<CategoriasServiceWCF>();

// Identity Core
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


var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CategoriasServiceWCF>(serviceOptions =>
    {
        serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });
    serviceBuilder.AddServiceEndpoint<CategoriasServiceWCF, ICategoriasServiceWCF>(
        new CoreWCF.BasicHttpBinding(),
        "/CategoriaService.svc"
    );
});

// Publicar metadatos (WSDL) para cliente SOAP
var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
serviceMetadataBehavior.HttpGetEnabled = true;

app.Run();
