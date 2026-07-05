using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class OccurrentParticipation<TOccurrentId> : AggregateRoot<ParticipationId>
{
    private AgentId _agentId = default!;
    private TOccurrentId _occurrentId = default!;

    protected OccurrentParticipation()
    {
    }

    protected OccurrentParticipation(ParticipationId id, AgentId agentId, TOccurrentId occurrentId)
    {
        Id = id;
        UpdateOccurrentParticipant(agentId, occurrentId);
    }

    protected void UpdateOccurrentParticipant(AgentId agentId, TOccurrentId occurrentId)
    {
        _agentId = agentId;
        _occurrentId = occurrentId;
    }
}
