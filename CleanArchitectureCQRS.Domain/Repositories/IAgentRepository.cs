using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Repositories;

public interface IAgentRepository<TAgent> where TAgent : Agent
{
    Task<TAgent?> GetAsync(AgentId id);
    Task AddAsync(TAgent agent);
    Task UpdateAsync(TAgent agent);
    Task DeleteAsync(TAgent agent);
}
