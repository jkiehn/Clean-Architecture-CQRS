using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public class Customer : ExternalAgent
{
    public Customer()
    {
    }

    public Customer(AgentId id, AgentName name, AgentEmail email) : base(id, name, email)
    {
    }
}
