namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal class EmployeeReadModel : AgentReadModelBase
{
    public string SocialSecurityNumber { get; set; } = string.Empty;
}
