using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record UpdateVendor(Guid Id, string Name, string Email) : ICommand, IUpdateAgentSubtypeCommand;
