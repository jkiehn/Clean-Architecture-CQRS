using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class CommitmentParticipation : OccurrentParticipation<CommitmentId>
{
    protected CommitmentParticipation()
    {
    }

    protected CommitmentParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }

    protected void UpdateParticipants(AgentId agentId, CommitmentId commitmentId)
        => UpdateOccurrentParticipant(agentId, commitmentId);
}
