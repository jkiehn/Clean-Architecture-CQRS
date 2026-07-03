using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record RemoveItem(Guid Id) : ICommand, IRemoveResourceSubtypeCommand;
