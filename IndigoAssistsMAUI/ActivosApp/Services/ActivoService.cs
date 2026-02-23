using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ActivosApp.Models;

namespace ActivosApp.Services;

public class ActivoService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SessionService _session;
    private readonly JsonSerializerOptions _jsonOptions;

    public ActivoService(IHttpClientFactory httpClientFactory, SessionService session)
    {
        _httpClientFactory = httpClientFactory;
        _session = session;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<ActivoResumenDto>> ObtenerTodosAsync()
    {
        var client = CreateClient();
        using var response = await client.GetAsync("activos/todos");

        if (!response.IsSuccessStatusCode)
        {
            return new List<ActivoResumenDto>();
        }

        var data = await response.Content.ReadFromJsonAsync<List<ActivoResumenDto>>(_jsonOptions);
        return data ?? new List<ActivoResumenDto>();
    }

    public async Task<ActivoResumenDto?> ObtenerPorIdAsync(int id)
    {
        var client = CreateClient();
        using var response = await client.GetAsync($"activos/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<ActivoResumenDto>(_jsonOptions);
    }

    public async Task<bool> CrearAsync(ActivoDto dto)
    {
        var client = CreateClient();
        using var response = await client.PostAsJsonAsync("activos", dto, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ActualizarAsync(int id, ActivoDto dto)
    {
        var client = CreateClient();
        using var response = await client.PutAsJsonAsync($"activos/{id}", dto, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<(bool ok, string? error)> EliminarAsync(int id)
    {
        var client = CreateClient();
        using var response = await client.DeleteAsync($"activos/{id}");

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return (true, null);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return (false, "No existe");
        }

        var error = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(error) ? "Error" : error);
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
