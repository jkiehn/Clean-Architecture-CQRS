namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal abstract class ParticipationReadModelBase
{
    public Guid Id { get; set; }
    public Guid AgentId { get; set; }
    public Guid EventId { get; set; }
}
