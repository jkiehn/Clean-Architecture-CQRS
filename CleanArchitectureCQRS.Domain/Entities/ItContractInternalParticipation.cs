using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class ItContractInternalParticipation : InternalCommitmentParticipation
{
    public ItContractInternalParticipation()
    {
    }

    public ItContractInternalParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }
}
