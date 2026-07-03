using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record CreateCustomer(Guid Id, string Name, string Email) : ICommand;
