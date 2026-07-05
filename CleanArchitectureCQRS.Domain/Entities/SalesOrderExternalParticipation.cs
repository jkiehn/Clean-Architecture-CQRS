using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SalesOrderExternalParticipation : ExternalCommitmentParticipation
{
    public SalesOrderExternalParticipation()
    {
    }

    public SalesOrderExternalParticipation(ParticipationId id, AgentId agentId, CommitmentId commitmentId)
        : base(id, agentId, commitmentId)
    {
    }
}
