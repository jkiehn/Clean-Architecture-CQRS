using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class CreateItemHandler : CreateResourceSubtypeHandlerBase<Item, CreateItem>
{
    public CreateItemHandler(IItemRepository repository, IItemReadService readService)
        : base(
            repository,
            readService,
            name => new ItemAlreadyExistsException(name),
            command => new Item(command.Id, command.Name))
    {
    }
}
