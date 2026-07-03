using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class InternalAgent : Agent
{
    protected InternalAgent()
    {
    }

    protected InternalAgent(AgentId id, AgentName name, AgentEmail email)
        : base(id, name, email)
    {
    }
}
