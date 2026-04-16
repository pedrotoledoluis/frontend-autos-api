using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutosWeb.Models;

namespace AutosWeb.Services;

public class AutosApiClient : IAutosApiClient
{
    private const string ResourcePath = "api/autos";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly ILogger<AutosApiClient> _logger;

    public AutosApiClient(HttpClient http, ILogger<AutosApiClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AutoViewModel>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _http.GetFromJsonAsync<List<AutoViewModel>>(ResourcePath, JsonOptions, ct);
        return list ?? new List<AutoViewModel>();
    }

    public async Task<AutoViewModel?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"{ResourcePath}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AutoViewModel>(JsonOptions, ct);
    }

    public async Task<AutoViewModel?> CreateAsync(AutoViewModel model, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ResourcePath, model, JsonOptions, ct);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("POST {Path} devolvió {Status}", ResourcePath, response.StatusCode);
            return null;
        }
        return await response.Content.ReadFromJsonAsync<AutoViewModel>(JsonOptions, ct);
    }

    public async Task<bool> UpdateAsync(int id, AutoViewModel model, CancellationToken ct = default)
    {
        var response = await _http.PutAsJsonAsync($"{ResourcePath}/{id}", model, JsonOptions, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"{ResourcePath}/{id}", ct);
        return response.IsSuccessStatusCode;
    }
}
