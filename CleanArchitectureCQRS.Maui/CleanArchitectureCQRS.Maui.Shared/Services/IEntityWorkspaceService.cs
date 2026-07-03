namespace CleanArchitectureCQRS.Maui.Shared.Services;

public interface IEntityWorkspaceService
{
    EntityDescriptor Descriptor { get; }
    Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "");
    Task<EntityDetail?> GetAsync(Guid id);
    Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values);
    Task<EntityOperationResult> UpdateAsync(Guid id, IReadOnlyDictionary<string, object?> values);
    Task<EntityOperationResult> DeleteAsync(Guid id);
    Task<EntityOperationResult> ExecuteCollectionActionAsync(Guid entityId, string collectionKey, string actionKey, IReadOnlyDictionary<string, object?> values);
    Task<EntityOperationResult> ExecuteCollectionItemActionAsync(Guid entityId, string collectionKey, string itemKey, string actionKey);
}
