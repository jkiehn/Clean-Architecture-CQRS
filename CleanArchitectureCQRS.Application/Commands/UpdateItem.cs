using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record UpdateItem(Guid Id, string Name) : ICommand, IUpdateResourceSubtypeCommand;
