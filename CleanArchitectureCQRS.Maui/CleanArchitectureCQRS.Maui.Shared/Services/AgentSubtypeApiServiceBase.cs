namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class AgentSubtypeApiServiceBase : ApiServiceBase
{
    private readonly string _resourceName;

    protected AgentSubtypeApiServiceBase(HttpClient httpClient, string resourceName) : base(httpClient)
    {
        _resourceName = resourceName;
    }

    public async Task<IReadOnlyList<AgentSubtypeModel>> GetAllAsync(string searchPhrase = "")
    {
        var query = string.IsNullOrWhiteSpace(searchPhrase)
            ? string.Empty
            : $"?searchPhrase={Uri.EscapeDataString(searchPhrase)}";

        return await GetListAsync<AgentSubtypeModel>($"api/{_resourceName}{query}");
    }

    public Task<AgentSubtypeModel?> GetByIdAsync(Guid id)
        => GetAsync<AgentSubtypeModel>($"api/{_resourceName}/{id}");

    public Task CreateAsync(CreateAgentSubtypeCommand command)
        => PostAsync($"api/{_resourceName}", command);

    public Task UpdateAsync(UpdateAgentSubtypeCommand command)
        => PutAsync($"api/{_resourceName}/{command.Id}", command);

    public Task DeleteAsync(Guid id)
        => DeleteAsync($"api/{_resourceName}/{id}");
}
