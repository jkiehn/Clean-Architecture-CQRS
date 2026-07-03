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
    private readonly DbSet<VendorReadModel> _vendors;

    public SearchAgentsHandler(ReadDbContext context)
    {
        _customers = context.Customers;
        _vendors = context.Vendors;
    }

    public async Task<IEnumerable<AgentDto>> HandleAsync(SearchAgents query)
    {
        var customers = await BuildQuery(_customers, "Customer", query.SearchPhrase).ToListAsync();
        var vendors = await BuildQuery(_vendors, "Vendor", query.SearchPhrase).ToListAsync();

        return customers
            .Concat(vendors)
            .OrderBy(agent => agent.Name)
            .ToList();
    }

    private static IQueryable<AgentDto> BuildQuery<TReadModel>(DbSet<TReadModel> agents, string agentType, string? searchPhrase)
        where TReadModel : AgentReadModelBase
    {
        var query = agents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            query = query.Where(agent =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(agent.Name, $"%{searchPhrase}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(agent.Email, $"%{searchPhrase}%"));
        }

        return query.Select(agent => new AgentDto(agent.Id, agent.Name, agent.Email, agentType));
    }
}
