namespace CleanArchitectureCQRS.Maui.Shared.Services;

public record ResourceSubtypeModel(Guid Id, string Name);
public record CreateResourceSubtypeCommand(Guid Id, string Name);
public record UpdateResourceSubtypeCommand(Guid Id, string Name);
