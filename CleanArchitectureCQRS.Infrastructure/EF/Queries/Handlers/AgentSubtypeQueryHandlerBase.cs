using System.Linq.Expressions;
using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal abstract class GetAgentSubtypeHandlerBase<TReadModel>
    where TReadModel : AgentReadModelBase
{
    private readonly DbSet<TReadModel> _agents;
    private readonly Expression<Func<TReadModel, AgentSubtypeDto>> _selector;

    protected GetAgentSubtypeHandlerBase(DbSet<TReadModel> agents, Expression<Func<TReadModel, AgentSubtypeDto>> selector)
    {
        _agents = agents;
        _selector = selector;
    }

    protected Task<AgentSubtypeDto?> GetAsync(Guid id)
        => _agents
            .Where(agent => agent.Id == id)
            .Select(_selector)
            .AsNoTracking()
            .SingleOrDefaultAsync();
}

internal abstract class SearchAgentSubtypeHandlerBase<TReadModel>
    where TReadModel : AgentReadModelBase
{
    private readonly DbSet<TReadModel> _agents;
    private readonly Expression<Func<TReadModel, AgentSubtypeDto>> _selector;

    protected SearchAgentSubtypeHandlerBase(DbSet<TReadModel> agents, Expression<Func<TReadModel, AgentSubtypeDto>> selector)
    {
        _agents = agents;
        _selector = selector;
    }

    protected async Task<IEnumerable<AgentSubtypeDto>> SearchAsync(string? searchPhrase)
    {
        var dbQuery = _agents.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchPhrase))
        {
            dbQuery = dbQuery.Where(agent =>
                Microsoft.EntityFrameworkCore.EF.Functions.Like(agent.Name, $"%{searchPhrase}%") ||
                Microsoft.EntityFrameworkCore.EF.Functions.Like(agent.Email, $"%{searchPhrase}%"));
        }

        return await dbQuery
            .OrderBy(agent => agent.Name)
            .Select(_selector)
            .AsNoTracking()
            .ToListAsync();
    }
}
