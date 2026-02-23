using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ActivosApp.Models;

namespace ActivosApp.Services;

public class UserService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SessionService _session;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserService(IHttpClientFactory httpClientFactory, SessionService session)
    {
        _httpClientFactory = httpClientFactory;
        _session = session;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<UserContextDto?> GetContextAsync()
    {
        var client = CreateClient();
        using var response = await client.GetAsync("usuarios/contexto");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<UserContextDto>(_jsonOptions);
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
