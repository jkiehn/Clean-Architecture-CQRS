using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Domain.Entities.Sales;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class RemoveCustomerHandler : RemoveAgentSubtypeHandlerBase<Customer, RemoveCustomer>
{
    public RemoveCustomerHandler(ICustomerRepository repository)
        : base(repository, id => new CustomerNotFoundException(id))
    {
    }
}
