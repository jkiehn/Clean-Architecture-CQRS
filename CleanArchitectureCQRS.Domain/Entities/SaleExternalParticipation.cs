using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SaleExternalParticipation : ExternalParticipation
{
    public SaleExternalParticipation()
    {
    }

    public SaleExternalParticipation(ParticipationId id, AgentId agentId, EventId eventId)
        : base(id, agentId, eventId)
    {
    }
}
