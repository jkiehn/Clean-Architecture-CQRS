using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class GetCustomerHandler : GetAgentSubtypeHandlerBase<CustomerReadModel>, IQueryHandler<GetCustomer, AgentSubtypeDto>
{
    public GetCustomerHandler(ReadDbContext context)
        : base(context.Customers, customer => new AgentSubtypeDto(customer.Id, customer.Name, customer.Email))
    {
    }

    public Task<AgentSubtypeDto?> HandleAsync(GetCustomer query) => GetAsync(query.Id);
}
