namespace CleanArchitectureCQRS.Application.DTOs;

public enum EntityWorkspaceFieldKindDto
{
    Text,
    Number,
    Select,
    DateTime
}

public enum EntityWorkspaceLookupValueSourceDto
{
    Id,
    Title,
    Subtitle
}

public record EntityWorkspacePropertyDto(string Label, string Value);

public record EntityWorkspaceFieldOptionDto(string Value, string Label);

public record EntityWorkspaceLookupDefinitionDto(
    string EntityKey,
    string ButtonLabel = "Lookup",
    string EmptyStateMessage = "No matches found.",
    EntityWorkspaceLookupValueSourceDto ValueSource = EntityWorkspaceLookupValueSourceDto.Id);

public record EntityWorkspaceFieldDefinitionDto(
    string Key,
    string Label,
    EntityWorkspaceFieldKindDto Kind = EntityWorkspaceFieldKindDto.Text,
    bool Required = false,
    string? Placeholder = null,
    object? DefaultValue = null,
    IReadOnlyList<EntityWorkspaceFieldOptionDto>? Options = null,
    EntityWorkspaceLookupDefinitionDto? Lookup = null);

public record EntityWorkspaceActionDefinitionDto(
    string Key,
    string Label,
    string ButtonClass,
    IReadOnlyList<EntityWorkspaceFieldDefinitionDto>? Fields = null);

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
    EntityWorkspaceActionDefinitionDto? CreateAction = null);

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
