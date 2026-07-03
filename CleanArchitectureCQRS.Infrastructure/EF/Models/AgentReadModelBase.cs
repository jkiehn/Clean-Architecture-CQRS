namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal abstract class AgentReadModelBase : ContinuantReadModelBase
{
    public string Email { get; set; } = string.Empty;
}
