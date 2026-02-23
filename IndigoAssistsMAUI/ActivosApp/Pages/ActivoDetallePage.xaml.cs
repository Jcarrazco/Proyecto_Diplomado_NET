using ActivosApp.Models;
using ActivosApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ActivosApp.Pages;

public partial class ActivoDetallePage : ContentPage
{
    private readonly ActivoService _activoService;
    private readonly NotificationService _notification;
    private readonly IServiceProvider _services;
    private readonly int _id;
    private ActivoResumenDto? _activo;
    private bool _isBusy;

    public ActivoDetallePage(
        ActivoService activoService,
        NotificationService notification,
        IServiceProvider services,
        int id)
    {
        InitializeComponent();
        _activoService = activoService;
        _notification = notification;
        _services = services;
        _id = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadActivoAsync();
    }

    private async Task LoadActivoAsync()
    {
        if (_isBusy)
        {
            return;
        }

        SetBusy(true);

        try
        {
            _activo = await _activoService.ObtenerPorIdAsync(_id);

            if (_activo == null)
            {
                await _notification.ShowToast("Activo no encontrado");
                await Navigation.PopAsync();
                return;
            }

            CodigoLabel.Text = $"Codigo: {_activo.Codigo}";
            NombreLabel.Text = $"Nombre: {_activo.Nombre}";
            MarcaLabel.Text = $"Marca: {_activo.Marca}";
            ModeloLabel.Text = $"Modelo: {_activo.Modelo}";
            SerieLabel.Text = $"Serie: {_activo.Serie}";
            PersonaLabel.Text = $"Asignado: {_activo.PersonaAsign}";
            UbicacionLabel.Text = $"Ubicacion: {_activo.Ubicacion}";
            StatusLabel.Text = $"Status: {_activo.StatusNombre}";
            DepartamentoLabel.Text = $"Departamento: {_activo.DepartamentoNombre}";
            TipoLabel.Text = $"Tipo: {_activo.TipoActivoNombre}";
            ProveedorLabel.Text = $"Proveedor: {_activo.ProveedorNombre}";
            CostoLabel.Text = $"Costo: {_activo.CostoCompra}";
            NotasLabel.Text = $"Notas: {_activo.Notas}";
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (_activo == null)
        {
            return;
        }

        var editPage = ActivatorUtilities.CreateInstance<ActivoEditPage>(_services, _id, _activo);
        await Navigation.PushModalAsync(new NavigationPage(editPage));
    }

    private async void OnBorrarClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlertAsync("Confirmar", "Deseas borrar este activo?", "Si", "No");

        if (!confirm)
        {
            return;
        }

        SetBusy(true);

        try
        {
            var result = await _activoService.EliminarAsync(_id);

            if (result.ok)
            {
                await _notification.ShowToast("Eliminado");
                await Navigation.PopAsync();
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
    }
}
