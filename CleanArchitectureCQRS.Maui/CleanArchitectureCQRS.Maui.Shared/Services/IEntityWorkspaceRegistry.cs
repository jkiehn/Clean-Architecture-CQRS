namespace CleanArchitectureCQRS.Maui.Shared.Services;

public interface IEntityWorkspaceRegistry
{
    IReadOnlyList<EntityDescriptor> Descriptors { get; }
    IReadOnlyList<EntityDescriptorGroup> Groups { get; }
    IEntityWorkspaceService Get(string key);
}
