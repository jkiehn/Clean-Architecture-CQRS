using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record UpdateCustomer(Guid Id, string Name, string Email) : ICommand;
