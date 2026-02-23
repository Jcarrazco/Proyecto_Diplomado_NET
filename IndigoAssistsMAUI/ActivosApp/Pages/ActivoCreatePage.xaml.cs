using ActivosApp.Models;
using ActivosApp.Services;

namespace ActivosApp.Pages;

public partial class ActivoCreatePage : ContentPage
{
    private readonly ActivoService _activoService;
    private readonly NotificationService _notification;
    private bool _isBusy;

    public ActivoCreatePage(ActivoService activoService, NotificationService notification)
    {
        InitializeComponent();
        _activoService = activoService;
        _notification = notification;

        FeAltaPicker.Date = DateTime.Today;
        FeCompraPicker.Date = DateTime.Today;
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (_isBusy)
        {
            return;
        }

        var codigo = CodigoEntry.Text?.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            await _notification.ShowToast("Codigo requerido");
            return;
        }

        SetBusy(true);

        try
        {
            var dto = BuildDto();
            var createresponse = await _activoService.CrearAsync(dto);

            if (createresponse)
            {
                await _notification.ShowToast("Creado");
                await NavigateBackAsync();
            }
            else
            {
                await _notification.ShowToast("No se pudo crear");
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private ActivoDto BuildDto()
    {
        double? costo = null;
        if (double.TryParse(CostoEntry.Text, out var costoValue))
        {
            costo = costoValue;
        }

        int? codificacion = null;
        if (int.TryParse(CodificacionEntry.Text, out var codValue))
        {
            codificacion = codValue;
        }

        var feCompra = FeCompraPicker.Date ?? DateTime.Today;
        var feAlta = FeAltaPicker.Date ?? DateTime.Today;

        return new ActivoDto
        {
            Codigo = CodigoEntry.Text?.Trim() ?? string.Empty,
            Nombre = NombreEntry.Text?.Trim(),
            Marca = MarcaEntry.Text?.Trim(),
            Modelo = ModeloEntry.Text?.Trim(),
            Serie = SerieEntry.Text?.Trim(),
            PersonaAsign = PersonaEntry.Text?.Trim(),
            Ubicacion = UbicacionEntry.Text?.Trim(),
            FeCompra = new DateTimeOffset(feCompra).ToUniversalTime(),
            FeAlta = new DateTimeOffset(feAlta).ToUniversalTime(),
            CostoCompra = costo,
            CodificacionComponentes = codificacion,
            TipoActivoNombre = TipoEntry.Text?.Trim(),
            DepartamentoNombre = DepartamentoEntry.Text?.Trim(),
            StatusNombre = StatusEntry.Text?.Trim(),
            ProveedorNombre = ProveedorEntry.Text?.Trim(),
            Notas = NotasEditor.Text?.Trim(),
            TieneSoftwareOP = SoftwareSwitch.IsToggled
        };
    }

    private async Task NavigateBackAsync()
    {
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
        else if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//activos");
        }
    }

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
        GuardarButton.IsEnabled = !isBusy;
    }
}
