using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Agent : Continuant<AgentId, AgentName>
{
    private AgentEmail _email = default!;

    protected Agent()
    {
    }

    protected Agent(AgentId id, AgentName name, AgentEmail email)
        : base(id, name)
    {
        _email = email;
    }

    public void UpdateDetails(AgentName name, AgentEmail email)
    {
        UpdateName(name);
        _email = email;
    }
}
