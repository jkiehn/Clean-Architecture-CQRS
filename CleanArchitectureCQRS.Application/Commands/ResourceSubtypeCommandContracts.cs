using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public interface ICreateResourceSubtypeCommand : ICommand
{
    Guid Id { get; }
    string Name { get; }
}

public interface IUpdateResourceSubtypeCommand : ICommand
{
    Guid Id { get; }
    string Name { get; }
}

public interface IRemoveResourceSubtypeCommand : ICommand
{
    Guid Id { get; }
}
