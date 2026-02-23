using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ActivosApp.Models;

namespace ActivosApp.Services;

public class AuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<LoginResponseDto?> LoginAsync(string email, string password)
    {
        var client = _httpClientFactory.CreateClient("Api");
        var request = new LoginRequestDto
        {
            UserName = email,
            Password = password
        };
        

        using var response = await client.PostAsJsonAsync("auth/login", request, _jsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<LoginResponseDto>(_jsonOptions);
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var client = _httpClientFactory.CreateClient("Api");
        var request = new LoginRequestDto
        {
            Email = email,
            Password = password
        };

        using var response = await client.PostAsJsonAsync("auth/register", request, _jsonOptions);
        return response.IsSuccessStatusCode;
    }
}
