using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Domain.Entities.Inventory;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class RemoveItemHandler : RemoveResourceSubtypeHandlerBase<Item, RemoveItem>
{
    public RemoveItemHandler(IItemRepository repository)
        : base(repository, id => new ItemNotFoundException(id))
    {
    }
}
