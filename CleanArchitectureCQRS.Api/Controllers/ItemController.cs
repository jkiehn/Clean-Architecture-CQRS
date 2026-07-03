using CleanArchitectureCQRS.Application.Commands;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Api.Controllers;

public class ItemController : ResourceSubtypeControllerBase<GetItem, SearchItems, CreateItem, UpdateItem, RemoveItem>
{
    public ItemController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        : base(
            commandDispatcher,
            queryDispatcher,
            (command, id) => command with { Id = id },
            id => new RemoveItem(id))
    {
    }
}
