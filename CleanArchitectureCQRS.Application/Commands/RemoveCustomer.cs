using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands;

public record RemoveCustomer(Guid Id) : ICommand;
