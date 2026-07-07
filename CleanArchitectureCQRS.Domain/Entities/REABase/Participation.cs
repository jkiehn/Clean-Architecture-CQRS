using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Participation : OccurrentParticipation<EventId>
{
    protected Participation()
    {
    }

    protected Participation(ParticipationId id, AgentId agentId, EventId eventId)
    {
        Id = id;
        UpdateParticipants(agentId, eventId);
    }

    protected void UpdateParticipants(AgentId agentId, EventId eventId)
        => UpdateOccurrentParticipant(agentId, eventId);
}
