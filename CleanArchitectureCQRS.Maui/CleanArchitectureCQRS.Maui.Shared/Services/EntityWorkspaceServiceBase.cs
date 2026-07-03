namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class EntityWorkspaceServiceBase : IEntityWorkspaceService
{
    public abstract EntityDescriptor Descriptor { get; }

    public abstract Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "");
    public abstract Task<EntityDetail?> GetAsync(Guid id);

    public virtual Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values)
        => Task.FromException<EntityOperationResult>(new NotSupportedException($"{Descriptor.DisplayName} creation is not supported."));

    public virtual Task<EntityOperationResult> UpdateAsync(Guid id, IReadOnlyDictionary<string, object?> values)
        => Task.FromException<EntityOperationResult>(new NotSupportedException($"{Descriptor.DisplayName} updates are not supported."));

    public virtual Task<EntityOperationResult> DeleteAsync(Guid id)
        => Task.FromException<EntityOperationResult>(new NotSupportedException($"{Descriptor.DisplayName} deletion is not supported."));

    public virtual Task<EntityOperationResult> ExecuteCollectionActionAsync(Guid entityId, string collectionKey, string actionKey, IReadOnlyDictionary<string, object?> values)
        => Task.FromException<EntityOperationResult>(new NotSupportedException($"{Descriptor.DisplayName} collection actions are not supported."));

    public virtual Task<EntityOperationResult> ExecuteCollectionItemActionAsync(Guid entityId, string collectionKey, string itemKey, string actionKey)
        => Task.FromException<EntityOperationResult>(new NotSupportedException($"{Descriptor.DisplayName} item actions are not supported."));

    protected static string GetRequiredString(IReadOnlyDictionary<string, object?> values, string key)
    {
        if (!values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value?.ToString()))
        {
            throw new InvalidOperationException($"Field '{key}' is required.");
        }

        return value.ToString()!.Trim();
    }

    protected static int GetRequiredInt(IReadOnlyDictionary<string, object?> values, string key)
    {
        var stringValue = GetRequiredString(values, key);

        if (!int.TryParse(stringValue, out var parsed))
        {
            throw new InvalidOperationException($"Field '{key}' must be a valid number.");
        }

        return parsed;
    }
}
