using ActivosApp.Services;

namespace ActivosApp.Pages;

public partial class BorrarActivoPage : ContentPage
{
    private readonly ActivoService _activoService;
    private readonly NotificationService _notification;
    private bool _isBusy;

    public BorrarActivoPage(ActivoService activoService, NotificationService notification)
    {
        InitializeComponent();
        _activoService = activoService;
        _notification = notification;
    }

    private async void OnBorrarClicked(object sender, EventArgs e)
    {
        if (_isBusy)
        {
            return;
        }

        if (!int.TryParse(IdEntry.Text, out var id))
        {
            await _notification.ShowToast("Id invalido");
            return;
        }

        var confirm = await DisplayAlertAsync("Confirmar", "Deseas borrar este activo?", "Si", "No");

        if (!confirm)
        {
            return;
        }

        SetBusy(true);

        try
        {
            var result = await _activoService.EliminarAsync(id);

            if (result.ok)
            {
                await _notification.ShowToast("Eliminado");
                IdEntry.Text = string.Empty;
            }
            else
            {
                await _notification.ShowToast("No se pudo eliminar");
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
        BorrarButton.IsEnabled = !isBusy;
    }
}
