namespace CleanArchitectureCQRS.Maui.Shared.Services;

public interface IEntityWorkspaceRegistry
{
    IReadOnlyList<EntityDescriptor> Descriptors { get; }
    IEntityWorkspaceService Get(string key);
}
