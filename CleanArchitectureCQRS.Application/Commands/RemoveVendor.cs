using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record RemoveVendor(Guid Id) : ICommand, IRemoveAgentSubtypeCommand;
