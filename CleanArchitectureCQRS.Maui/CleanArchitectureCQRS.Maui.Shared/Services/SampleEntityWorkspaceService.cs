namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class SampleEntityWorkspaceService : EntityWorkspaceServiceBase
{
    private static readonly EntityActionDefinition CreateAction = new(
        "create",
        "Create entity",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Entity name"),
            new EntityFieldDefinition("gender", "Gender", EntityFieldKind.Select, DefaultValue: Gender.Male.ToString(), Options: Enum.GetValues<Gender>().Select(value => new EntityFieldOption(value.ToString(), value.ToString())).ToArray()),
            new EntityFieldDefinition("city", "City", Required: true, Placeholder: "City"),
            new EntityFieldDefinition("country", "Country", Required: true, Placeholder: "Country")
        });

    private static readonly EntityActionDefinition AddItemAction = new(
        "add-item",
        "Add item",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Item name", Required: true, Placeholder: "Item name"),
            new EntityFieldDefinition("quantity", "Quantity", EntityFieldKind.Number, Required: true, DefaultValue: 1)
        });

    private readonly SampleEntityService _service;

    public SampleEntityWorkspaceService(SampleEntityService service)
    {
        _service = service;
    }

    public override EntityDescriptor Descriptor { get; } = new(
        "sample-entities",
        "Sample Entity",
        "Sample Entities",
        "Manage destinations and item workflows for sample entities.",
        "bi-list-nested-nav-menu",
        "Search sample entities by name",
        "No sample entities found yet. Create the first one from the panel on the right.",
        10,
        CreateAction,
        DeleteLabel: "Delete");

    public override async Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "")
    {
        var entities = await _service.GetAllAsync(searchPhrase);

        return entities
            .Select(entity => new EntityListItem(
                entity.Id,
                entity.Name,
                string.IsNullOrWhiteSpace(entity.DestinationLabel) ? "Destination unavailable" : entity.DestinationLabel,
                new[]
                {
                    new EntityProperty("Items", $"{entity.SafeItems.Count()} item(s)")
                }))
            .ToArray();
    }

    public override async Task<EntityDetail?> GetAsync(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);

        if (entity is null)
        {
            return null;
        }

        return new EntityDetail(
            entity.Id,
            entity.Name,
            string.IsNullOrWhiteSpace(entity.DestinationLabel) ? "Destination unavailable" : entity.DestinationLabel,
            new[]
            {
                new EntityProperty("Destination", string.IsNullOrWhiteSpace(entity.DestinationLabel) ? "Destination unavailable" : entity.DestinationLabel),
                new EntityProperty("Identifier", entity.Id.ToString())
            },
            new Dictionary<string, object?>(),
            new[]
            {
                new EntityCollectionSection(
                    "items",
                    "Items",
                    "No items yet for this entity.",
                    entity.SafeItems.Select(item => new EntityCollectionItem(
                        item.Name,
                        item.Name,
                        item.IsTaken ? "Taken" : "Available",
                        new[]
                        {
                            new EntityProperty("Quantity", $"{item.Quantity} in stock")
                        },
                        GetItemActions(item))).ToArray(),
                    AddItemAction)
            });
    }

    public override async Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values)
    {
        var id = Guid.NewGuid();
        var genderValue = GetRequiredString(values, "gender");
        var gender = Enum.TryParse<Gender>(genderValue, true, out var parsedGender)
            ? parsedGender
            : Gender.Male;

        var command = new CreateSampleEntityCommand(
            id,
            GetRequiredString(values, "name"),
            gender,
            new DestinationWriteModel(GetRequiredString(values, "city"), GetRequiredString(values, "country")));

        await _service.CreateAsync(command);
        return new EntityOperationResult("Entity created.", id);
    }

    public override async Task<EntityOperationResult> DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
        return new EntityOperationResult("Entity deleted.");
    }

    public override async Task<EntityOperationResult> ExecuteCollectionActionAsync(Guid entityId, string collectionKey, string actionKey, IReadOnlyDictionary<string, object?> values)
    {
        if (!string.Equals(collectionKey, "items", StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(actionKey, "add-item", StringComparison.OrdinalIgnoreCase))
        {
            return await base.ExecuteCollectionActionAsync(entityId, collectionKey, actionKey, values);
        }

        await _service.AddItemAsync(entityId, GetRequiredString(values, "name"), (uint)GetRequiredInt(values, "quantity"));
        return new EntityOperationResult("Item added.");
    }

    public override async Task<EntityOperationResult> ExecuteCollectionItemActionAsync(Guid entityId, string collectionKey, string itemKey, string actionKey)
    {
        if (!string.Equals(collectionKey, "items", StringComparison.OrdinalIgnoreCase))
        {
            return await base.ExecuteCollectionItemActionAsync(entityId, collectionKey, itemKey, actionKey);
        }

        if (string.Equals(actionKey, "take", StringComparison.OrdinalIgnoreCase))
        {
            await _service.TakeItemAsync(entityId, itemKey);
            return new EntityOperationResult("Item marked as taken.");
        }

        if (string.Equals(actionKey, "remove", StringComparison.OrdinalIgnoreCase))
        {
            await _service.DeleteItemAsync(entityId, itemKey);
            return new EntityOperationResult("Item removed.");
        }

        return await base.ExecuteCollectionItemActionAsync(entityId, collectionKey, itemKey, actionKey);
    }

    private static IReadOnlyList<EntityCollectionItemAction> GetItemActions(SampleEntityItemModel item)
    {
        var actions = new List<EntityCollectionItemAction>();

        if (!item.IsTaken)
        {
            actions.Add(new EntityCollectionItemAction("take", "Mark taken", "btn btn-sm btn-outline-success"));
        }

        actions.Add(new EntityCollectionItemAction("remove", "Remove", "btn btn-sm btn-outline-danger"));
        return actions;
    }
}
