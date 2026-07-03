using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class UpdateCustomerHandler : ICommandHandler<UpdateCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly ICustomerReadService _readService;

    public UpdateCustomerHandler(ICustomerRepository repository, ICustomerReadService readService)
    {
        _repository = repository;
        _readService = readService;
    }

    public async Task HandleAsync(UpdateCustomer command)
    {
        var customer = await _repository.GetAsync(command.Id);

        if (customer is null)
        {
            throw new CustomerNotFoundException(command.Id);
        }

        if (await _readService.ExistsByEmailAsync(command.Email, command.Id))
        {
            throw new CustomerAlreadyExistsException(command.Email);
        }

        customer.UpdateDetails(command.Name, command.Email);
        await _repository.UpdateAsync(customer);
    }
}
