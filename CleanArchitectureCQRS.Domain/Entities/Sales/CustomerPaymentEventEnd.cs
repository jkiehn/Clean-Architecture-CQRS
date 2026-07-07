using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class CustomerPaymentEventEnd : EventStockflowEnd
{
    public CustomerPaymentEventEnd()
    {
    }

    public CustomerPaymentEventEnd(StockflowEndId id, StockflowId stockflowId, EventId eventId)
        : base(id, stockflowId, eventId)
    {
    }
}
