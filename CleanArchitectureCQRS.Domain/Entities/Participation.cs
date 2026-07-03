using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Participation : AggregateRoot<ParticipationId>
{
    private AgentId _agentId = default!;
    private EventId _eventId = default!;

    protected Participation()
    {
    }

    protected Participation(ParticipationId id, AgentId agentId, EventId eventId)
    {
        Id = id;
        UpdateParticipants(agentId, eventId);
    }

    protected void UpdateParticipants(AgentId agentId, EventId eventId)
    {
        _agentId = agentId;
        _eventId = eventId;
    }
}
