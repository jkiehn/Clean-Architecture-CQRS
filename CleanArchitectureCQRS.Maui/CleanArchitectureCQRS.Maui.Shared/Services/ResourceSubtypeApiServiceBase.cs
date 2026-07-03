namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class ResourceSubtypeApiServiceBase : ApiServiceBase
{
    private readonly string _resourceName;

    protected ResourceSubtypeApiServiceBase(HttpClient httpClient, string resourceName) : base(httpClient)
    {
        _resourceName = resourceName;
    }

    public async Task<IReadOnlyList<ResourceSubtypeModel>> GetAllAsync(string searchPhrase = "")
    {
        var query = string.IsNullOrWhiteSpace(searchPhrase)
            ? string.Empty
            : $"?searchPhrase={Uri.EscapeDataString(searchPhrase)}";

        return await GetListAsync<ResourceSubtypeModel>($"api/{_resourceName}{query}");
    }

    public Task<ResourceSubtypeModel?> GetByIdAsync(Guid id)
        => GetAsync<ResourceSubtypeModel>($"api/{_resourceName}/{id}");

    public Task CreateAsync(CreateResourceSubtypeCommand command)
        => PostAsync($"api/{_resourceName}", command);

    public Task UpdateAsync(UpdateResourceSubtypeCommand command)
        => PutAsync($"api/{_resourceName}/{command.Id}", command);

    public Task DeleteAsync(Guid id)
        => DeleteAsync($"api/{_resourceName}/{id}");
}
