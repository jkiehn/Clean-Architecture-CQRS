using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class CreateCustomerHandler : CreateAgentSubtypeHandlerBase<Customer, CreateCustomer>
{
    public CreateCustomerHandler(ICustomerRepository repository, ICustomerReadService readService)
        : base(
            repository,
            readService,
            email => new CustomerAlreadyExistsException(email),
            command => new Customer(command.Id, command.Name, command.Email))
    {
    }
}
