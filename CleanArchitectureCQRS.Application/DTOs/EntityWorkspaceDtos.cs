namespace CleanArchitectureCQRS.Application.DTOs;

public record EntityWorkspacePropertyDto(string Label, string Value);

public record EntityWorkspaceCollectionItemActionDto(string Key, string Label, string ButtonClass);

public record EntityWorkspaceCollectionItemDto(
    string Key,
    string Title,
    string? Subtitle,
    IReadOnlyList<EntityWorkspacePropertyDto> Meta,
    IReadOnlyList<EntityWorkspaceCollectionItemActionDto> Actions);

public record EntityWorkspaceCollectionSectionDto(
    string Key,
    string Title,
    string EmptyStateMessage,
    IReadOnlyList<EntityWorkspaceCollectionItemDto> Items,
    object? CreateAction = null);

public record EntityWorkspaceListItemDto(
    Guid Id,
    string Title,
    string? Subtitle,
    IReadOnlyList<EntityWorkspacePropertyDto> Meta);

public record EntityWorkspaceDetailDto(
    Guid Id,
    string Title,
    string? Subtitle,
    IReadOnlyList<EntityWorkspacePropertyDto> Summary,
    IReadOnlyDictionary<string, object?> EditValues,
    IReadOnlyList<EntityWorkspaceCollectionSectionDto> Collections);

public record EntityWorkspaceOperationResultDto(string Message, Guid? EntityId = null);
