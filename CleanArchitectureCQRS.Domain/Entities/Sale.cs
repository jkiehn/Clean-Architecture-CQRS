using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public class Sale : Event
{
    private ParticipationId _internalParticipationId = default!;
    private AgentId _employeeId = default!;
    private ParticipationId _externalParticipationId = default!;
    private AgentId _customerId = default!;

    public Sale()
    {
    }

    public Sale(EventId id, DateTimeOffset when, AgentId employeeId, AgentId customerId, DateTimeOffset? endWhen = null, decimal? amount = null)
        : base(id, when, endWhen, amount)
    {
        _internalParticipationId = new ParticipationId(Guid.NewGuid());
        _externalParticipationId = new ParticipationId(Guid.NewGuid());
        UpdateCounterparties(employeeId, customerId);
    }

    public void UpdateDetails(DateTimeOffset when, AgentId employeeId, AgentId customerId, DateTimeOffset? endWhen = null, decimal? amount = null)
    {
        UpdateTemporalDetails(when, endWhen, amount);
        UpdateCounterparties(employeeId, customerId);
    }

    public SaleInternalParticipation GetInternalParticipation()
        => new(_internalParticipationId, _employeeId, Id);

    public SaleExternalParticipation GetExternalParticipation()
        => new(_externalParticipationId, _customerId, Id);

    private void UpdateCounterparties(AgentId employeeId, AgentId customerId)
    {
        _employeeId = employeeId;
        _customerId = customerId;
    }
}
