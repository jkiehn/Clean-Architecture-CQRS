namespace CleanArchitectureCQRS.Maui.Shared.Services;

public class SampleEntityService : ApiServiceBase
{
    public SampleEntityService(HttpClient httpClient) : base(httpClient)
    {
    }

    public Task<IReadOnlyList<SampleEntityModel>> GetAllAsync(string searchPhrase = "")
    {
        var query = string.IsNullOrWhiteSpace(searchPhrase)
            ? string.Empty
            : $"?searchPhrase={Uri.EscapeDataString(searchPhrase)}";

        return GetListAsync<SampleEntityModel>($"api/SampleEntity{query}");
    }

    public Task<SampleEntityModel?> GetByIdAsync(Guid id)
        => GetAsync<SampleEntityModel>($"api/SampleEntity/{id}");

    public Task CreateAsync(CreateSampleEntityCommand command)
        => PostAsync("api/SampleEntity", command);

    public Task AddItemAsync(Guid sampleEntityId, string name, uint quantity)
        => PutAsync($"api/SampleEntity/{sampleEntityId}/items", new { SampleEntityId = sampleEntityId, Name = name, Quantity = quantity });

    public Task TakeItemAsync(Guid sampleEntityId, string name)
        => PutAsync($"api/SampleEntity/{sampleEntityId}/items/{Uri.EscapeDataString(name)}/Take", new { SampleEntityId = sampleEntityId, Name = name });

    public Task DeleteItemAsync(Guid sampleEntityId, string name)
        => DeleteAsync($"api/SampleEntity/{sampleEntityId}/items/{Uri.EscapeDataString(name)}", new { SampleEntityId = sampleEntityId, Name = name });

    public Task DeleteAsync(Guid id)
        => DeleteAsync($"api/SampleEntity/{id}", new { Id = id });
}
