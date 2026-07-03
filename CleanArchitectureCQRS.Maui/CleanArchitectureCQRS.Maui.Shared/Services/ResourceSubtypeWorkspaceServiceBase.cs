namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class ResourceSubtypeWorkspaceServiceBase : EntityWorkspaceServiceBase
{
    private static readonly EntityActionDefinition CreateAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Resource name")
        });

    private static readonly EntityActionDefinition EditAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Resource name")
        });

    private readonly ResourceSubtypeApiServiceBase _service;
    private readonly string _resourceType;

    protected ResourceSubtypeWorkspaceServiceBase(ResourceSubtypeApiServiceBase service, EntityDescriptor descriptor, string resourceType)
    {
        _service = service;
        _resourceType = resourceType;
        Descriptor = descriptor with
        {
            CreateAction = descriptor.CreateAction ?? CreateAction,
            EditAction = descriptor.EditAction ?? EditAction,
            DeleteLabel = descriptor.DeleteLabel ?? "Delete"
        };
    }

    public override EntityDescriptor Descriptor { get; }

    public override async Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "")
    {
        var resources = await _service.GetAllAsync(searchPhrase);

        return resources
            .Select(resource => new EntityListItem(
                resource.Id,
                resource.Name,
                null,
                new[]
                {
                    new EntityProperty("Type", _resourceType)
                }))
            .ToArray();
    }

    public override async Task<EntityDetail?> GetAsync(Guid id)
    {
        var resource = await _service.GetByIdAsync(id);

        if (resource is null)
        {
            return null;
        }

        return new EntityDetail(
            resource.Id,
            resource.Name,
            null,
            new[]
            {
                new EntityProperty("Identifier", resource.Id.ToString()),
                new EntityProperty("Type", _resourceType)
            },
            new Dictionary<string, object?>
            {
                ["name"] = resource.Name
            },
            Array.Empty<EntityCollectionSection>());
    }

    public override async Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values)
    {
        var id = Guid.NewGuid();
        await _service.CreateAsync(new CreateResourceSubtypeCommand(id, GetRequiredString(values, "name")));
        return new EntityOperationResult($"{Descriptor.DisplayName} created.", id);
    }

    public override async Task<EntityOperationResult> UpdateAsync(Guid id, IReadOnlyDictionary<string, object?> values)
    {
        await _service.UpdateAsync(new UpdateResourceSubtypeCommand(id, GetRequiredString(values, "name")));
        return new EntityOperationResult($"{Descriptor.DisplayName} updated.", id);
    }

    public override async Task<EntityOperationResult> DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
        return new EntityOperationResult($"{Descriptor.DisplayName} deleted.");
    }
}
