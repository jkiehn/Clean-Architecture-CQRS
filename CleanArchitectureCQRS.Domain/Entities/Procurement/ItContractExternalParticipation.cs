using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Procurement;

public sealed class ItContractExternalParticipation : ExternalCommitmentParticipation
{
    public ItContractExternalParticipation()
    {
    }

    public ItContractExternalParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }
}
