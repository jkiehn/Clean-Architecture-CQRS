using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public class Customer : Agent
{
    public Customer()
    {
    }

    public Customer(AgentId id, AgentName name, AgentEmail email) : base(id, name, email)
    {
    }
}
