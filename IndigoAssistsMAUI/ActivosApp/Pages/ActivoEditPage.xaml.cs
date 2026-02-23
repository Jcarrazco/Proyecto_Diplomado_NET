using ActivosApp.Models;
using ActivosApp.Services;

namespace ActivosApp.Pages;

public partial class ActivoEditPage : ContentPage
{
    private readonly ActivoService _activoService;
    private readonly NotificationService _notification;
    private readonly int _id;
    private ActivoResumenDto? _activo;
    private bool _isBusy;

    public ActivoEditPage(
        ActivoService activoService,
        NotificationService notification,
        int id,
        ActivoResumenDto? activo)
    {
        InitializeComponent();
        _activoService = activoService;
        _notification = notification;
        _id = id;
        _activo = activo;

        FeAltaPicker.Date = DateTime.Today;
        FeCompraPicker.Date = DateTime.Today;
        LoadFromActivo();
    }

    private void LoadFromActivo()
    {
        if (_activo == null)
        {
            return;
        }

        CodigoEntry.Text = _activo.Codigo;
        NombreEntry.Text = _activo.Nombre;
        MarcaEntry.Text = _activo.Marca;
        ModeloEntry.Text = _activo.Modelo;
        SerieEntry.Text = _activo.Serie;
        PersonaEntry.Text = _activo.PersonaAsign;
        UbicacionEntry.Text = _activo.Ubicacion;
        TipoEntry.Text = _activo.TipoActivoNombre;
        DepartamentoEntry.Text = _activo.DepartamentoNombre;
        StatusEntry.Text = _activo.StatusNombre;
        ProveedorEntry.Text = _activo.ProveedorNombre;
        CostoEntry.Text = _activo.CostoCompra?.ToString();
        CodificacionEntry.Text = _activo.CodificacionComponentes?.ToString();
        NotasEditor.Text = _activo.Notas;
        SoftwareSwitch.IsToggled = _activo.TieneSoftwareOP ?? false;

        if (_activo.FeCompra.HasValue)
        {
            FeCompraPicker.Date = _activo.FeCompra.Value.Date;
        }

        FeAltaPicker.Date = _activo.FeAlta.Date;
    }

    private async void OnActualizarClicked(object sender, EventArgs e)
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
            var ok = await _activoService.ActualizarAsync(_id, dto);

            if (ok)
            {
                await _notification.ShowToast("Actualizado");
                await Navigation.PopModalAsync();
            }
            else
            {
                await _notification.ShowToast("No se pudo actualizar");
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

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
        ActualizarButton.IsEnabled = !isBusy;
    }
}
