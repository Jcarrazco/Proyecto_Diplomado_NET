using System.Linq;
using ActivosApp.Models;
using ActivosApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ActivosApp.Pages;

public partial class ActivosVistaPage : ContentPage
{
    private readonly ActivoService _activoService;
    private readonly NotificationService _notification;
    private readonly IServiceProvider _services;
    private bool _isBusy;

    public ActivosVistaPage(
        ActivoService activoService,
        NotificationService notification,
        IServiceProvider services)
    {
        InitializeComponent();
        _activoService = activoService;
        _notification = notification;
        _services = services;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadActivosAsync();
    }

    private async Task LoadActivosAsync()
    {
        if (_isBusy)
        {
            return;
        }

        SetBusy(true);

        try
        {
            var activos = await _activoService.ObtenerTodosAsync();
            ActivosCollection.ItemsSource = activos;
        }
        catch
        {
            await _notification.ShowToast("No se pudo cargar la lista");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not ActivoResumenDto activo)
        {
            return;
        }

        ActivosCollection.SelectedItem = null;

        var page = ActivatorUtilities.CreateInstance<ActivoDetallePage>(_services, activo.IdActivo);
        await Navigation.PushAsync(page);
    }

    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<ActivoCreatePage>();
        await Navigation.PushAsync(page);
    }

    private async void OnBorrarClicked(object sender, EventArgs e)
    {
        var page = _services.GetRequiredService<BorrarActivoPage>();
        await Navigation.PushAsync(page);
    }

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
    }
}
