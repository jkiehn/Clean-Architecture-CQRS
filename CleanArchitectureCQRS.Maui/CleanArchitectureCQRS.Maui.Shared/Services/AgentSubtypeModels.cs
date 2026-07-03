namespace CleanArchitectureCQRS.Maui.Shared.Services;

public record AgentSubtypeModel(Guid Id, string Name, string Email);
public record CreateAgentSubtypeCommand(Guid Id, string Name, string Email);
public record UpdateAgentSubtypeCommand(Guid Id, string Name, string Email);
