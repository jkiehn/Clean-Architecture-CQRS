using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class RemoveCustomerHandler : ICommandHandler<RemoveCustomer>
{
    private readonly ICustomerRepository _repository;

    public RemoveCustomerHandler(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RemoveCustomer command)
    {
        var customer = await _repository.GetAsync(command.Id);

        if (customer is null)
        {
            throw new CustomerNotFoundException(command.Id);
        }

        await _repository.DeleteAsync(customer);
    }
}
