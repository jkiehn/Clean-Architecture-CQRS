using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Services;

internal class AgentSubtypeReadService<TReadModel> : IAgentSubtypeReadService
    where TReadModel : AgentReadModelBase
{
    private readonly DbSet<TReadModel> _agents;

    protected AgentSubtypeReadService(DbSet<TReadModel> agents)
    {
        _agents = agents;
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeAgentId = null)
        => _agents.AnyAsync(agent =>
            agent.Email == email && (!excludeAgentId.HasValue || agent.Id != excludeAgentId.Value));
}
