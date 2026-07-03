using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Agent : AggregateRoot<AgentId>
{
    public AgentId Id { get; protected set; } = default!;
    private AgentName _name = default!;
    private AgentEmail _email = default!;

    protected Agent()
    {
    }

    protected Agent(AgentId id, AgentName name, AgentEmail email)
    {
        Id = id;
        _name = name;
        _email = email;
    }

    public void UpdateDetails(AgentName name, AgentEmail email)
    {
        _name = name;
        _email = email;
    }
}
