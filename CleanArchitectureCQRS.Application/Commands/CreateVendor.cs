using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record CreateVendor(Guid Id, string Name, string Email) : ICommand, ICreateAgentSubtypeCommand;
