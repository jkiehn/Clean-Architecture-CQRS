using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class CustomerPaymentExternalParticipation : ExternalParticipation
{
    public CustomerPaymentExternalParticipation()
    {
    }

    public CustomerPaymentExternalParticipation(ParticipationId id, AgentId agentId, EventId eventId)
        : base(id, agentId, eventId)
    {
    }
}
