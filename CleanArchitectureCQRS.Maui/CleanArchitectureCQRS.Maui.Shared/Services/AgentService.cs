namespace CleanArchitectureCQRS.Maui.Shared.Services;

public class AgentService : ApiServiceBase
{
    public AgentService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IReadOnlyList<AgentModel>> GetAllAsync(string searchPhrase = "")
    {
        var query = string.IsNullOrWhiteSpace(searchPhrase)
            ? string.Empty
            : $"?searchPhrase={Uri.EscapeDataString(searchPhrase)}";

        return await GetListAsync<AgentModel>($"api/Agent{query}");
    }
}
