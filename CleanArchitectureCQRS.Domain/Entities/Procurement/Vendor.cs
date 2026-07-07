using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Procurement;

public class Vendor : ExternalAgent
{
    public Vendor()
    {
    }

    public Vendor(AgentId id, AgentName name, AgentEmail email) : base(id, name, email)
    {
    }
}
