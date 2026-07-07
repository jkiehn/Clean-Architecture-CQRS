using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class ExternalAgent : Agent
{
    protected ExternalAgent()
    {
    }

    protected ExternalAgent(AgentId id, AgentName name, AgentEmail email)
        : base(id, name, email)
    {
    }
}
