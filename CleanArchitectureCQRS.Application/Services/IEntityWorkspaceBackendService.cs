using CleanArchitectureCQRS.Application.DTOs;

namespace CleanArchitectureCQRS.Application.Services;

public interface IEntityWorkspaceBackendService
{
    Task<IReadOnlyList<EntityWorkspaceListItemDto>> SearchAsync(string entityKey, string? searchPhrase = null);
    Task<EntityWorkspaceDetailDto?> GetAsync(string entityKey, Guid id);
    Task<EntityWorkspaceOperationResultDto> CreateAsync(string entityKey, IReadOnlyDictionary<string, string?> values);
    Task<EntityWorkspaceOperationResultDto> UpdateAsync(string entityKey, Guid id, IReadOnlyDictionary<string, string?> values);
    Task<EntityWorkspaceOperationResultDto> DeleteAsync(string entityKey, Guid id);
    Task<EntityWorkspaceOperationResultDto> ExecuteCollectionActionAsync(string entityKey, Guid id, string collectionKey, string actionKey, IReadOnlyDictionary<string, string?> values);
    Task<EntityWorkspaceOperationResultDto> ExecuteCollectionItemActionAsync(string entityKey, Guid id, string collectionKey, string itemKey, string actionKey);
}
