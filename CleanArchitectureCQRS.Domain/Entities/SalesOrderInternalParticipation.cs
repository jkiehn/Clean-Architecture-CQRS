using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SalesOrderInternalParticipation : InternalCommitmentParticipation
{
    public SalesOrderInternalParticipation()
    {
    }

    public SalesOrderInternalParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }
}
