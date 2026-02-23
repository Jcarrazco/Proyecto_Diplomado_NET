using ActivosApp.Pages;
using ActivosApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ActivosApp;

public partial class AppShell : Shell
{
    private readonly IServiceProvider _services;
    private readonly SessionService _session;
    private readonly NotificationService _notification;
    private readonly UserService _userService;
    private bool _initialized;

    public AppShell(
        IServiceProvider services,
        SessionService session,
        NotificationService notification,
        UserService userService)
    {
        InitializeComponent();
        _services = services;
        _session = session;
        _notification = notification;
        _userService = userService;

        BuildShellItems();

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Logout",
            Order = ToolbarItemOrder.Primary,
            Priority = 0,
            Command = new Command(async () => await LogoutAsync())
        });
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_initialized)
        {
            return;
        }

        _initialized = true;
        await EnsureContextAsync();
        BuildShellItems();
    }

    private async Task EnsureContextAsync()
    {
        if (!_session.IsLoggedIn || _session.UserContext != null)
        {
            return;
        }

        var context = await _userService.GetContextAsync();
        if (context != null)
        {
            _session.SetUserContext(context);
        }
    }

    private void BuildShellItems()
    {
        Items.Clear();

        var tabBar = new TabBar();
        tabBar.Items.Add(CreateTab("Activos", "activos", _services.GetRequiredService<ActivosVistaPage>()));
        ShellContent? ticketsTab = null;

        if (_session.IsTecnico || _session.IsAdministrador)
        {
            ticketsTab = CreateTab("Tickets", "tickets", _services.GetRequiredService<TicketsDashboardPage>());
            tabBar.Items.Add(ticketsTab);
        }

        Items.Add(tabBar);

        if (_session.IsAdministrador && ticketsTab != null)
        {
            tabBar.CurrentItem = ticketsTab;
        }

        if (Items.Count > 0 && CurrentItem == null)
        {
            CurrentItem = Items[0];
        }
    }

    private ShellContent CreateTab(string title, string route, Page page)
    {
        return new ShellContent
        {
            Title = title,
            Route = route,
            Content = page
        };
    }

    private async Task LogoutAsync()
    {
        _session.Logout();
        await _notification.ShowToast("Sesion cerrada");

        if (Application.Current is App app)
        {
            app.SetMainPageLoggedOut();
        }
    }
}
