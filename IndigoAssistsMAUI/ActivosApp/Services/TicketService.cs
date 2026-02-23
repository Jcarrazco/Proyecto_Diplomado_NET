using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ActivosApp.Models;

namespace ActivosApp.Services;

public class TicketService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SessionService _session;
    private readonly JsonSerializerOptions _jsonOptions;

    public TicketService(IHttpClientFactory httpClientFactory, SessionService session)
    {
        _httpClientFactory = httpClientFactory;
        _session = session;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<TicketDashboardDto?> GetDashboardAsync(string scope)
    {
        var client = CreateClient();
        var scopeValue = string.IsNullOrWhiteSpace(scope) ? "depto" : scope;
        using var response = await client.GetAsync($"tickets/dashboard?scope={Uri.EscapeDataString(scopeValue)}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TicketDashboardDto>(_jsonOptions);
    }

    public async Task<TicketResponseDto?> GetByIdAsync(int id)
    {
        var client = CreateClient();
        using var response = await client.GetAsync($"tickets/{id}");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<TicketResponseDto>(_jsonOptions);
    }

    public async Task<bool> AgregarAnotacionAsync(TicketAnotacionCreateDto dto)
    {
        var client = CreateClient();
        using var response = await client.PostAsJsonAsync($"tickets/{dto.IdTicket}/anotaciones", dto, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CerrarAsync(int id)
    {
        var client = CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"tickets/{id}/cerrar");
        using var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ReabrirAsync(int id)
    {
        var client = CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"tickets/{id}/reabrir");
        using var response = await client.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient("Api");

        if (!string.IsNullOrWhiteSpace(_session.Token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _session.Token);
        }

        return client;
    }
}
