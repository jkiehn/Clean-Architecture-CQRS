namespace CleanArchitectureCQRS.Maui.Shared.Services;

public enum EntityFieldKind
{
    Text,
    Number,
    Select,
    DateTime
}

public enum EntityLookupValueSource
{
    Id,
    Title,
    Subtitle
}

public record EntityFieldOption(string Value, string Label);

public record EntityLookupDefinition(
    string EntityKey,
    string ButtonLabel = "Lookup",
    string EmptyStateMessage = "No matches found.",
    EntityLookupValueSource ValueSource = EntityLookupValueSource.Id);

public record EntityFieldDefinition(
    string Key,
    string Label,
    EntityFieldKind Kind = EntityFieldKind.Text,
    bool Required = false,
    string? Placeholder = null,
    object? DefaultValue = null,
    IReadOnlyList<EntityFieldOption>? Options = null,
    EntityLookupDefinition? Lookup = null);

public record EntityActionDefinition(
    string Key,
    string Label,
    string ButtonClass,
    IReadOnlyList<EntityFieldDefinition>? Fields = null);

public record EntityDescriptor(
    string Key,
    string DisplayName,
    string PluralDisplayName,
    string Description,
    string IconCssClass,
    string SearchPlaceholder,
    string EmptyStateMessage,
    int Order,
    EntityActionDefinition? CreateAction = null,
    EntityActionDefinition? EditAction = null,
    string? DeleteLabel = null);

public record EntityProperty(string Label, string Value);

public record EntityListItem(Guid Id, string Title, string? Subtitle, IReadOnlyList<EntityProperty> Meta);

public record EntityCollectionItemAction(string Key, string Label, string ButtonClass);

public record EntityCollectionItem(
    string Key,
    string Title,
    string? Subtitle,
    IReadOnlyList<EntityProperty> Meta,
    IReadOnlyList<EntityCollectionItemAction> Actions);

public record EntityCollectionSection(
    string Key,
    string Title,
    string EmptyStateMessage,
    IReadOnlyList<EntityCollectionItem> Items,
    EntityActionDefinition? CreateAction = null);

public record EntityDetail(
    Guid Id,
    string Title,
    string? Subtitle,
    IReadOnlyList<EntityProperty> Summary,
    IReadOnlyDictionary<string, object?> EditValues,
    IReadOnlyList<EntityCollectionSection> Collections);

public record EntityOperationResult(string Message, Guid? EntityId = null);
