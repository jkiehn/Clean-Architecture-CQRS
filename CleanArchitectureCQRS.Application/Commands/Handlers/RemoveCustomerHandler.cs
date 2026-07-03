using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class RemoveCustomerHandler : RemoveAgentSubtypeHandlerBase<CleanArchitectureCQRS.Domain.Entities.Customer, RemoveCustomer>
{
    public RemoveCustomerHandler(ICustomerRepository repository)
        : base(repository, id => new CustomerNotFoundException(id))
    {
    }
}
