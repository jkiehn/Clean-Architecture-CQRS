namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class DescriptorDrivenWorkspaceService : EntityWorkspaceServiceBase
{
    private readonly DescriptorEntityApiService _service;

    public DescriptorDrivenWorkspaceService(DescriptorEntityApiService service, EntityDescriptor descriptor)
    {
        _service = service;
        Descriptor = descriptor;
    }

    public override EntityDescriptor Descriptor { get; }

    public override Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "")
        => _service.GetAllAsync(Descriptor.Key, searchPhrase);

    public override Task<EntityDetail?> GetAsync(Guid id)
        => _service.GetByIdAsync(Descriptor.Key, id);

    public override async Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values)
    {
        if (Descriptor.CreateAction is null)
        {
            return await base.CreateAsync(values);
        }

        return await _service.CreateAsync(Descriptor.Key, values)
            ?? new EntityOperationResult($"{Descriptor.DisplayName} created.");
    }

    public override async Task<EntityOperationResult> UpdateAsync(Guid id, IReadOnlyDictionary<string, object?> values)
    {
        if (Descriptor.EditAction is null)
        {
            return await base.UpdateAsync(id, values);
        }

        return await _service.UpdateAsync(Descriptor.Key, id, values)
            ?? new EntityOperationResult($"{Descriptor.DisplayName} updated.", id);
    }

    public override async Task<EntityOperationResult> DeleteAsync(Guid id)
    {
        if (string.IsNullOrWhiteSpace(Descriptor.DeleteLabel))
        {
            return await base.DeleteAsync(id);
        }

        return await _service.DeleteAsync(Descriptor.Key, id)
            ?? new EntityOperationResult($"{Descriptor.DisplayName} deleted.");
    }
}
