using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities.Sales;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class UpdateCustomerHandler : UpdateAgentSubtypeHandlerBase<Customer, UpdateCustomer>
{
    public UpdateCustomerHandler(ICustomerRepository repository, ICustomerReadService readService)
        : base(
            repository,
            readService,
            email => new CustomerAlreadyExistsException(email),
            id => new CustomerNotFoundException(id))
    {
    }
}
