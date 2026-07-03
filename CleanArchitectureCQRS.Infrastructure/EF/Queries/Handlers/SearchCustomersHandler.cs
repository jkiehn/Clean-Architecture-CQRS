using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class SearchCustomersHandler : SearchAgentSubtypeHandlerBase<CustomerReadModel>, IQueryHandler<SearchCustomers, IEnumerable<AgentSubtypeDto>>
{
    public SearchCustomersHandler(ReadDbContext context)
        : base(context.Customers, customer => new AgentSubtypeDto(customer.Id, customer.Name, customer.Email))
    {
    }

    public Task<IEnumerable<AgentSubtypeDto>> HandleAsync(SearchCustomers query) => SearchAsync(query.SearchPhrase);
}
