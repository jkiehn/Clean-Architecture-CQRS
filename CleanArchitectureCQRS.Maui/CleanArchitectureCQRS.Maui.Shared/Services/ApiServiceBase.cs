using System.Net.Http.Json;

namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class ApiServiceBase
{
    private readonly HttpClient _httpClient;
    private readonly ILocalePreferenceService _localePreferenceService;

    protected ApiServiceBase(HttpClient httpClient, ILocalePreferenceService localePreferenceService)
    {
        _httpClient = httpClient;
        _localePreferenceService = localePreferenceService;
    }

    protected async Task<T?> GetAsync<T>(string uri)
    {
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<IReadOnlyList<T>> GetListAsync<T>(string uri)
    {
        using var request = CreateRequest(HttpMethod.Get, uri);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<IReadOnlyList<T>>() ?? Array.Empty<T>();
    }

    protected async Task PostAsync<T>(string uri, T payload)
    {
        using var request = CreateRequest(HttpMethod.Post, uri, payload);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
    }

    protected async Task PutAsync<T>(string uri, T payload)
    {
        using var request = CreateRequest(HttpMethod.Put, uri, payload);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
    }

    protected async Task DeleteAsync(string uri)
    {
        using var request = CreateRequest(HttpMethod.Delete, uri);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
    }

    protected async Task<T?> PostAndReadAsync<TPayload, T>(string uri, TPayload payload)
    {
        using var request = CreateRequest(HttpMethod.Post, uri, payload);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<T?> PutAndReadAsync<TPayload, T>(string uri, TPayload payload)
    {
        using var request = CreateRequest(HttpMethod.Put, uri, payload);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task<T?> DeleteAndReadAsync<T>(string uri)
    {
        using var request = CreateRequest(HttpMethod.Delete, uri);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    protected async Task DeleteAsync<T>(string uri, T payload)
    {
        using var request = CreateRequest(HttpMethod.Delete, uri, payload);
        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string uri, object? payload = null)
    {
        var request = new HttpRequestMessage(method, uri);
        request.Headers.TryAddWithoutValidation(LocalePreferences.HeaderName, _localePreferenceService.CurrentCultureName);

        if (payload is not null)
        {
            request.Content = JsonContent.Create(payload);
        }

        return request;
    }

    protected static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync();
        var message = string.IsNullOrWhiteSpace(details)
            ? $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase})."
            : $"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}). Response: {details}";

        throw new HttpRequestException(message, null, response.StatusCode);
    }
}
