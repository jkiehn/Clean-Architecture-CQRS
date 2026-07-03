using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class UpdateItemHandler : UpdateResourceSubtypeHandlerBase<Item, UpdateItem>
{
    public UpdateItemHandler(IItemRepository repository, IItemReadService readService)
        : base(
            repository,
            readService,
            name => new ItemAlreadyExistsException(name),
            id => new ItemNotFoundException(id))
    {
    }
}
