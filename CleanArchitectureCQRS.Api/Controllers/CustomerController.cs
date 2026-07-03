using CleanArchitectureCQRS.Application.Commands;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Api.Controllers;

public class CustomerController : AgentSubtypeControllerBase<GetCustomer, SearchCustomers, CreateCustomer, UpdateCustomer, RemoveCustomer>
{
    public CustomerController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        : base(
            commandDispatcher,
            queryDispatcher,
            (command, id) => command with { Id = id },
            id => new RemoveCustomer(id))
    {
    }
}
