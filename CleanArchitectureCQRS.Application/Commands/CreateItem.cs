using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record CreateItem(Guid Id, string Name) : ICommand, ICreateResourceSubtypeCommand;
