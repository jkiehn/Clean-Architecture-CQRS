namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class DescriptorEntityApiService : ApiServiceBase
{
    public DescriptorEntityApiService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<IReadOnlyList<EntityListItem>> GetAllAsync(string entityKey, string searchPhrase = "")
    {
        var query = string.IsNullOrWhiteSpace(searchPhrase)
            ? string.Empty
            : $"?searchPhrase={Uri.EscapeDataString(searchPhrase)}";

        return await GetListAsync<EntityListItem>($"api/entities/{entityKey}{query}");
    }

    public Task<EntityDetail?> GetByIdAsync(string entityKey, Guid id)
        => GetAsync<EntityDetail>($"api/entities/{entityKey}/{id}");

    public Task<EntityOperationResult?> CreateAsync(string entityKey, IReadOnlyDictionary<string, object?> values)
        => PostAndReadAsync<DescriptorEntityMutationRequest, EntityOperationResult>($"api/entities/{entityKey}", new DescriptorEntityMutationRequest(ToStringValues(values)));

    public Task<EntityOperationResult?> UpdateAsync(string entityKey, Guid id, IReadOnlyDictionary<string, object?> values)
        => PutAndReadAsync<DescriptorEntityMutationRequest, EntityOperationResult>($"api/entities/{entityKey}/{id}", new DescriptorEntityMutationRequest(ToStringValues(values)));

    public Task<EntityOperationResult?> DeleteAsync(string entityKey, Guid id)
        => DeleteAndReadAsync<EntityOperationResult>($"api/entities/{entityKey}/{id}");

    public Task<EntityOperationResult?> ExecuteCollectionActionAsync(string entityKey, Guid entityId, string collectionKey, string actionKey, IReadOnlyDictionary<string, object?> values)
        => PostAndReadAsync<DescriptorEntityMutationRequest, EntityOperationResult>(
            $"api/entities/{entityKey}/{entityId}/collections/{collectionKey}/actions/{actionKey}",
            new DescriptorEntityMutationRequest(ToStringValues(values)));

    public Task<EntityOperationResult?> ExecuteCollectionItemActionAsync(string entityKey, Guid entityId, string collectionKey, string itemKey, string actionKey)
        => PostAndReadAsync<object, EntityOperationResult>(
            $"api/entities/{entityKey}/{entityId}/collections/{collectionKey}/items/{Uri.EscapeDataString(itemKey)}/actions/{actionKey}",
            new { });

    private static IReadOnlyDictionary<string, string?> ToStringValues(IReadOnlyDictionary<string, object?> values)
        => values.ToDictionary(pair => pair.Key, pair => pair.Value?.ToString(), StringComparer.OrdinalIgnoreCase);
}

public record DescriptorEntityMutationRequest(IReadOnlyDictionary<string, string?> Values);
