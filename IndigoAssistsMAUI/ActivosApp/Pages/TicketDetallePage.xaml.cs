using System;
using ActivosApp.Models;
using ActivosApp.Services;

namespace ActivosApp.Pages;

public partial class TicketDetallePage : ContentPage
{
    private readonly TicketService _ticketService;
    private readonly SessionService _session;
    private readonly NotificationService _notification;
    private readonly int _id;
    private TicketResponseDto? _ticket;
    private bool _isBusy;

    public TicketDetallePage(
        TicketService ticketService,
        SessionService session,
        NotificationService notification,
        int id)
    {
        InitializeComponent();
        _ticketService = ticketService;
        _session = session;
        _notification = notification;
        _id = id;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTicketAsync();
    }

    private async Task LoadTicketAsync(bool ignoreBusy = false)
    {
        if (_isBusy && !ignoreBusy)
        {
            return;
        }

        SetBusy(true);

        try
        {
            _ticket = await _ticketService.GetByIdAsync(_id);
            if (_ticket == null)
            {
                await _notification.ShowToast("Ticket no encontrado");
                await Navigation.PopAsync();
                return;
            }

            IdLabel.Text = $"Ticket #{_ticket.IdTicket}";
            TituloLabel.Text = _ticket.Titulo;
            StatusLabel.Text = $"Status: {_ticket.StatusDescripcion}";
            DeptoLabel.Text = $"Depto: {_ticket.DepartamentoNombre}";
            SolicitanteLabel.Text = $"Solicitante: {_ticket.SolicitanteNombre}";
            TecnicoLabel.Text = $"Tecnico: {_ticket.TecnicoNombre}";
            CategoriaLabel.Text = $"Categoria: {_ticket.CategoriaNombre}";
            SubCategoriaLabel.Text = $"Subcategoria: {_ticket.SubCategoriaNombre}";
            PrioridadLabel.Text = $"Prioridad: {_ticket.PrioridadNombre}";
            FeAltaLabel.Text = $"Fecha alta: {_ticket.FeAlta:g}";
            DescripcionLabel.Text = _ticket.Descripcion;

            var cerrado = IsClosed(_ticket);
            CerrarButton.IsVisible = !cerrado;
            ReabrirButton.IsVisible = cerrado;
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnAgregarAnotacionClicked(object sender, EventArgs e)
    {
        if (_isBusy || _ticket == null)
        {
            return;
        }

        var texto = AnotacionEditor.Text?.Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            await _notification.ShowToast("Anotacion requerida");
            return;
        }

        var usuario = _session.UserContext?.Login ?? _session.UsuarioNombre;
        if (string.IsNullOrWhiteSpace(usuario))
        {
            await _notification.ShowToast("Usuario no disponible");
            return;
        }

        SetBusy(true);

        try
        {
            var ok = await _ticketService.AgregarAnotacionAsync(new TicketAnotacionCreateDto
            {
                IdTicket = _ticket.IdTicket,
                Observacion = texto,
                Duracion = 0,
                Usuario = usuario
            });

            if (ok)
            {
                AnotacionEditor.Text = string.Empty;
                await _notification.ShowToast("Anotacion guardada");
            }
            else
            {
                await _notification.ShowToast("No se pudo guardar la anotacion");
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnCerrarClicked(object sender, EventArgs e)
    {
        if (_isBusy || _ticket == null)
        {
            return;
        }

        var confirm = await DisplayAlertAsync("Confirmar", "Deseas cerrar este ticket?", "Si", "No");
        if (!confirm)
        {
            return;
        }

        SetBusy(true);

        try
        {
            var ok = await _ticketService.CerrarAsync(_ticket.IdTicket);
            if (ok)
            {
                await _notification.ShowToast("Ticket cerrado");
                await LoadTicketAsync(true);
            }
            else
            {
                await _notification.ShowToast("No se pudo cerrar");
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnReabrirClicked(object sender, EventArgs e)
    {
        if (_isBusy || _ticket == null)
        {
            return;
        }

        var confirm = await DisplayAlertAsync("Confirmar", "Deseas reabrir este ticket?", "Si", "No");
        if (!confirm)
        {
            return;
        }

        SetBusy(true);

        try
        {
            var ok = await _ticketService.ReabrirAsync(_ticket.IdTicket);
            if (ok)
            {
                await _notification.ShowToast("Ticket reabierto");
                await LoadTicketAsync(true);
            }
            else
            {
                await _notification.ShowToast("No se pudo reabrir");
            }
        }
        finally
        {
            SetBusy(false);
        }
    }

    private static bool IsClosed(TicketResponseDto ticket)
    {
        if (ticket.Status >= 3)
        {
            return true;
        }

        return ticket.StatusDescripcion.Contains("cerr", StringComparison.OrdinalIgnoreCase);
    }

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        LoadingIndicator.IsVisible = isBusy;
        LoadingIndicator.IsRunning = isBusy;
        AgregarAnotacionButton.IsEnabled = !isBusy;
        CerrarButton.IsEnabled = !isBusy;
        ReabrirButton.IsEnabled = !isBusy;
    }
}
