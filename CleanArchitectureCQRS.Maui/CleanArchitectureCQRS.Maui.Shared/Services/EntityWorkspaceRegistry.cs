namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class EntityWorkspaceRegistry : IEntityWorkspaceRegistry
{
    private readonly IReadOnlyDictionary<string, IEntityWorkspaceService> _services;

    public EntityWorkspaceRegistry(IEnumerable<IEntityWorkspaceService> services)
    {
        var orderedServices = services
            .OrderBy(service => service.Descriptor.Order)
            .ToArray();

        Descriptors = orderedServices
            .Select(service => service.Descriptor)
            .ToArray();

        _services = orderedServices.ToDictionary(service => service.Descriptor.Key, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyList<EntityDescriptor> Descriptors { get; }

    public IEntityWorkspaceService Get(string key)
    {
        if (_services.TryGetValue(key, out var service))
        {
            return service;
        }

        throw new KeyNotFoundException($"No entity workspace service was registered for '{key}'.");
    }
}
