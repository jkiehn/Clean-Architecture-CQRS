using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class ExternalParticipation : Participation
{
    protected ExternalParticipation()
    {
    }

    protected ExternalParticipation(ParticipationId id, AgentId agentId, EventId eventId)
        : base(id, agentId, eventId)
    {
    }
}
