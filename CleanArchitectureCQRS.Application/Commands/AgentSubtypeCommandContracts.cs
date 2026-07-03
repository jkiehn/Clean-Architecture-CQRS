using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public interface ICreateAgentSubtypeCommand : ICommand
{
    Guid Id { get; }
    string Name { get; }
    string Email { get; }
}

public interface IUpdateAgentSubtypeCommand : ICommand
{
    Guid Id { get; }
    string Name { get; }
    string Email { get; }
}

public interface IRemoveAgentSubtypeCommand : ICommand
{
    Guid Id { get; }
}
