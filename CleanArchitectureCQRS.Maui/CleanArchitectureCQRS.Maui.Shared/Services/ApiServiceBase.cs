using System.Net.Http.Json;

namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class ApiServiceBase
{
    private readonly HttpClient _httpClient;

    protected ApiServiceBase(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<T?> GetAsync<T>(string uri)
        => await _httpClient.GetFromJsonAsync<T>(uri);

    protected async Task<IReadOnlyList<T>> GetListAsync<T>(string uri)
        => await _httpClient.GetFromJsonAsync<IReadOnlyList<T>>(uri) ?? Array.Empty<T>();

    protected async Task PostAsync<T>(string uri, T payload)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, payload);
        await EnsureSuccessAsync(response);
    }

    protected async Task PutAsync<T>(string uri, T payload)
    {
        var response = await _httpClient.PutAsJsonAsync(uri, payload);
        await EnsureSuccessAsync(response);
    }

    protected async Task DeleteAsync(string uri)
    {
        var response = await _httpClient.DeleteAsync(uri);
        await EnsureSuccessAsync(response);
    }

    protected async Task DeleteAsync<T>(string uri, T payload)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, uri)
        {
            Content = JsonContent.Create(payload)
        });

        await EnsureSuccessAsync(response);
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
