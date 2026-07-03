using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class CreateCustomerHandler : ICommandHandler<CreateCustomer>
{
    private readonly ICustomerRepository _repository;
    private readonly ICustomerReadService _readService;

    public CreateCustomerHandler(ICustomerRepository repository, ICustomerReadService readService)
    {
        _repository = repository;
        _readService = readService;
    }

    public async Task HandleAsync(CreateCustomer command)
    {
        if (await _readService.ExistsByEmailAsync(command.Email))
        {
            throw new CustomerAlreadyExistsException(command.Email);
        }

        var customer = new Customer(command.Id, command.Name, command.Email);
        await _repository.AddAsync(customer);
    }
}
