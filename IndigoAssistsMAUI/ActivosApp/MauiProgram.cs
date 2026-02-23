using ActivosApp.Pages;
using ActivosApp.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ActivosApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options =>
                {
                    options.SetShouldEnableSnackbarOnWindows(true);
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<SessionService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<ActivoService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<TicketService>();
            builder.Services.AddSingleton<NotificationService>();

            builder.Services.AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri("http://31.97.131.142/api/");
            });

            builder.Services.AddTransient<AppShell>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ActivosVistaPage>();
            builder.Services.AddTransient<ActivoDetallePage>();
            builder.Services.AddTransient<ActivoCreatePage>();
            builder.Services.AddTransient<ActivoEditPage>();
            builder.Services.AddTransient<BorrarActivoPage>();
            builder.Services.AddTransient<TicketsDashboardPage>();
            builder.Services.AddTransient<TicketDetallePage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

			return builder.Build();
        }
    }
}
