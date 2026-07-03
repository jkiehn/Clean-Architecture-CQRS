using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class InternalParticipation : Participation
{
    protected InternalParticipation()
    {
    }

    protected InternalParticipation(ParticipationId id, AgentId agentId, EventId eventId)
        : base(id, agentId, eventId)
    {
    }
}
