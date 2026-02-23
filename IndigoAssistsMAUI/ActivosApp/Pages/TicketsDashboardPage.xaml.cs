using System.Linq;
using ActivosApp.Models;
using ActivosApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ActivosApp.Pages;

public partial class TicketsDashboardPage : ContentPage
{
    public static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(
        nameof(IsLoading),
        typeof(bool),
        typeof(TicketsDashboardPage),
        false);

    private readonly TicketService _ticketService;
    private readonly NotificationService _notification;
    private readonly IServiceProvider _services;
    private bool _isBusy;

    public bool IsLoading
    {
        get => (bool)GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public TicketsDashboardPage(
        TicketService ticketService,
        NotificationService notification,
        IServiceProvider services)
    {
        InitializeComponent();
        _ticketService = ticketService;
        _notification = notification;
        _services = services;
        SetActiveTab("AbiertosDepto");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        if (_isBusy)
        {
            return;
        }

        SetBusy(true);

        try
        {
            var deptoTask = _ticketService.GetDashboardAsync("depto");
            var usuarioTask = _ticketService.GetDashboardAsync("usuario");

            var deptoDashboard = await deptoTask;
            var usuarioDashboard = await usuarioTask;

            AbiertosDeptoCollection.ItemsSource =
                deptoDashboard?.Listas.AbiertosDepto ?? new List<TicketResponseDto>();
            EnProcesoDeptoCollection.ItemsSource =
                deptoDashboard?.Listas.EnProcesoDepto ?? new List<TicketResponseDto>();
            AbiertosMiosCollection.ItemsSource =
                usuarioDashboard?.Listas.AbiertosUsuario ?? new List<TicketResponseDto>();
            EnProcesoMiosCollection.ItemsSource =
                usuarioDashboard?.Listas.EnProcesoUsuario ?? new List<TicketResponseDto>();
        }
        catch
        {
            await _notification.ShowToast("No se pudieron cargar los tickets");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnTicketSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TicketResponseDto ticket)
        {
            return;
        }

        if (sender is CollectionView collection)
        {
            collection.SelectedItem = null;
        }

        var page = ActivatorUtilities.CreateInstance<TicketDetallePage>(_services, ticket.IdTicket);
        await Navigation.PushAsync(page);
    }

    private void SetBusy(bool isBusy)
    {
        _isBusy = isBusy;
        IsLoading = isBusy;
    }

    private void OnTabClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string tabKey)
        {
            SetActiveTab(tabKey);
        }
    }

    private void SetActiveTab(string tabKey)
    {
        AbiertosDeptoCollection.IsVisible = tabKey == "AbiertosDepto";
        EnProcesoDeptoCollection.IsVisible = tabKey == "EnProcesoDepto";
        AbiertosMiosCollection.IsVisible = tabKey == "AbiertosMios";
        EnProcesoMiosCollection.IsVisible = tabKey == "EnProcesoMios";

        UpdateTabButtonStyles(tabKey);
    }

    private void UpdateTabButtonStyles(string activeTab)
    {
        SetTabButtonStyle(AbiertosDeptoTabButton, activeTab == "AbiertosDepto");
        SetTabButtonStyle(EnProcesoDeptoTabButton, activeTab == "EnProcesoDepto");
        SetTabButtonStyle(AbiertosMiosTabButton, activeTab == "AbiertosMios");
        SetTabButtonStyle(EnProcesoMiosTabButton, activeTab == "EnProcesoMios");
    }

    private static void SetTabButtonStyle(Button button, bool isActive)
    {
        button.BackgroundColor = isActive ? Colors.Black : Colors.White;
        button.TextColor = isActive ? Colors.White : Colors.Black;
        button.BorderColor = Colors.Black;
        button.BorderWidth = 1;
        button.CornerRadius = 10;
        button.Padding = new Thickness(8, 6);
    }
}
