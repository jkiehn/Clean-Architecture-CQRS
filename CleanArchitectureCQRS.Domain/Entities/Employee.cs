using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public class Employee : Agent
{
    private string _socialSecurityNumber = string.Empty;

    public Employee()
    {
    }

    public Employee(AgentId id, AgentName name, AgentEmail email, string socialSecurityNumber) : base(id, name, email)
    {
        _socialSecurityNumber = ValidateSocialSecurityNumber(socialSecurityNumber);
    }

    public void UpdateDetails(AgentName name, AgentEmail email, string socialSecurityNumber)
    {
        base.UpdateDetails(name, email);
        _socialSecurityNumber = ValidateSocialSecurityNumber(socialSecurityNumber);
    }

    private static string ValidateSocialSecurityNumber(string socialSecurityNumber)
    {
        if (string.IsNullOrWhiteSpace(socialSecurityNumber))
        {
            throw new InvalidOperationException("Social security number is required.");
        }

        return socialSecurityNumber.Trim();
    }
}
