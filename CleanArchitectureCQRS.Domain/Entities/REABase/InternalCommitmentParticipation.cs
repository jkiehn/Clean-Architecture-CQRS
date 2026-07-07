using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class InternalCommitmentParticipation : CommitmentParticipation
{
    protected InternalCommitmentParticipation()
    {
    }

    protected InternalCommitmentParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }
}
