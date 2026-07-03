using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Repositories;

internal class AgentRepository<TAgent> : IAgentRepository<TAgent> where TAgent : Agent
{
    private readonly DbSet<TAgent> _agents;
    private readonly WriteDbContext _writeDbContext;

    protected AgentRepository(WriteDbContext writeDbContext, DbSet<TAgent> agents)
    {
        _agents = agents;
        _writeDbContext = writeDbContext;
    }

    public Task<TAgent?> GetAsync(AgentId id)
        => _agents.SingleOrDefaultAsync(agent => agent.Id == id);

    public async Task AddAsync(TAgent agent)
    {
        await _agents.AddAsync(agent);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(TAgent agent)
    {
        _agents.Update(agent);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(TAgent agent)
    {
        _agents.Remove(agent);
        await _writeDbContext.SaveChangesAsync();
    }
}
