using ActivosApp.Services;

namespace ActivosApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;
    private readonly SessionService _session;
    private readonly NotificationService _notification;
    private readonly UserService _userService;
    private bool _isBusy;

    public LoginPage(
        AuthService authService,
        SessionService session,
        NotificationService notification,
        UserService userService)
    {
        InitializeComponent();
        _authService = authService;
        _session = session;
        _notification = notification;
        _userService = userService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isBusy)
        {
            return;
        }

        var userName = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text ?? string.Empty;

        if (!ValidateCredentials(userName, password))
        {
            return;
        }

        SetBusy(true);

        try
        {
            var safeUserName = userName!;
            var result = await _authService.LoginAsync(safeUserName, password);

            if (result != null && !string.IsNullOrWhiteSpace(result.AccessToken))
            {
                _session.SetSession(result.AccessToken, safeUserName);

                var context = await _userService.GetContextAsync();
                if (context != null)
                {
                    _session.SetUserContext(context);
                }
                await _notification.ShowToast($"Bienvenido {_session.UsuarioNombre}");

                if (Application.Current is App app)
                {
                    app.SetMainPageLoggedIn();
                }
            }
            else
            {
                await _notification.ShowToast("Credenciales invalidas");
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await _notification.ShowToast("Registro no disponible");
    }

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
        IngresarButton.IsEnabled = !isBusy;
        RegistrarButton.IsEnabled = !isBusy;
    }

    private bool ValidateCredentials(string? userName, string password)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            _ = _notification.ShowToast("Usuario requerido");
            return false;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            _ = _notification.ShowToast("Password requerido");
            return false;
        }

        if (password.Length < 6)
        {
            _ = _notification.ShowToast("Password minimo 6 caracteres");
            return false;
        }

        return true;
    }

}
