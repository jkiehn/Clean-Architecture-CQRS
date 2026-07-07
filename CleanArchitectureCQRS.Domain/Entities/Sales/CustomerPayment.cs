using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class CustomerPayment : Event
{
    private ParticipationId _externalParticipationId = default!;
    private AgentId _customerId = default!;
    private StockflowId _cashFlowId = default!;
    private ResourceId _cashResourceId = default!;

    public CustomerPayment()
    {
    }

    public CustomerPayment(EventId id, DateTimeOffset when, AgentId customerId, ResourceId cashResourceId, decimal amount, DateTimeOffset? endWhen = null)
        : base(id, when, endWhen, amount)
    {
        _externalParticipationId = new ParticipationId(Guid.NewGuid());
        _cashFlowId = new StockflowId(Guid.NewGuid());
        UpdateDetails(when, customerId, cashResourceId, amount, endWhen);
    }

    public void UpdateDetails(DateTimeOffset when, AgentId customerId, ResourceId cashResourceId, decimal amount, DateTimeOffset? endWhen = null)
    {
        if (amount <= 0)
        {
            throw new EventInvalidException();
        }

        UpdateTemporalDetails(when, endWhen, amount);
        _customerId = customerId;
        _cashResourceId = cashResourceId;
    }

    public CustomerPaymentExternalParticipation GetExternalParticipation()
        => new(_externalParticipationId, _customerId, Id);

    public CustomerPaymentCashFlow GetCashFlow()
        => new(_cashFlowId, Id, _cashResourceId);
}
