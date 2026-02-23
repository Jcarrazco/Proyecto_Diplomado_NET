using System.Linq;
using ActivosApp.Pages;
using ActivosApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ActivosApp
{
public partial class App : Application
{
    private readonly IServiceProvider _services;
    private readonly SessionService _session;
    private Window? _mainWindow;

    public App(SessionService session, IServiceProvider services)
    {
        InitializeComponent();
        _session = session;
        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var startPage = _session.IsLoggedIn
            ? (Page)_services.GetRequiredService<AppShell>()
            : (Page)_services.GetRequiredService<LoginPage>();

        _mainWindow = new Window(startPage);
        return _mainWindow;
    }

    public void SetMainPageLoggedIn()
    {
        var window = _mainWindow ?? Windows.FirstOrDefault();
        if (window is not null)
        {
            window.Page = _services.GetRequiredService<AppShell>();
        }
    }

    public void SetMainPageLoggedOut()
    {
        var window = _mainWindow ?? Windows.FirstOrDefault();
        if (window is not null)
        {
            window.Page = _services.GetRequiredService<LoginPage>();
        }
    }
}
}
