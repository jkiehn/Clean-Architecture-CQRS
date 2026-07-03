using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class SearchAgentsHandler : IQueryHandler<SearchAgents, IEnumerable<AgentDto>>
{
    private readonly DbSet<CustomerReadModel> _customers;

    public SearchAgentsHandler(ReadDbContext context)
    {
        _customers = context.Customers;
    }

    public async Task<IEnumerable<AgentDto>> HandleAsync(SearchAgents query)
    {
        var dbQuery = _customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.SearchPhrase))
        {
            dbQuery = dbQuery.Where(customer =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(customer.Name, $"%{query.SearchPhrase}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(customer.Email, $"%{query.SearchPhrase}%"));
        }

        return await dbQuery
            .OrderBy(customer => customer.Name)
            .Select(customer => customer.AsAgentDto())
            .AsNoTracking()
            .ToListAsync();
    }
}
