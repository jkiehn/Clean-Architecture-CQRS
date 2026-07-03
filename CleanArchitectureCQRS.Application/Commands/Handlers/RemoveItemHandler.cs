using CleanArchitectureCQRS.Application.Exceptions;
using CleanArchitectureCQRS.Domain.Repositories;

namespace CleanArchitectureCQRS.Application.Commands.Handlers;

internal sealed class RemoveItemHandler : RemoveResourceSubtypeHandlerBase<CleanArchitectureCQRS.Domain.Entities.Item, RemoveItem>
{
    public RemoveItemHandler(IItemRepository repository)
        : base(repository, id => new ItemNotFoundException(id))
    {
    }
}
