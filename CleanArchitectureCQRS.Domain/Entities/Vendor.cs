using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public class Vendor : Agent
{
    public Vendor()
    {
    }

    public Vendor(AgentId id, AgentName name, AgentEmail email) : base(id, name, email)
    {
    }
}
