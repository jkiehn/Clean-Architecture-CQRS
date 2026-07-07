using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public class Customer : ExternalAgent
{
    public Customer()
    {
    }

    public Customer(AgentId id, AgentName name, AgentEmail email) : base(id, name, email)
    {
    }
}
