using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SaleInternalParticipation : InternalParticipation
{
    public SaleInternalParticipation()
    {
    }

    public SaleInternalParticipation(ParticipationId id, AgentId agentId, EventId eventId)
        : base(id, agentId, eventId)
    {
    }
}
