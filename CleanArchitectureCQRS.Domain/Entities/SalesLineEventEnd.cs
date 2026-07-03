using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SalesLineEventEnd : EventStockflowEnd
{
    public SalesLineEventEnd()
    {
    }

    public SalesLineEventEnd(StockflowEndId id, StockflowId stockflowId, EventId eventId)
        : base(id, stockflowId, eventId)
    {
    }
}
