using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class ExternalCommitmentParticipation : CommitmentParticipation
{
    protected ExternalCommitmentParticipation()
    {
    }

    protected ExternalCommitmentParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }
}
