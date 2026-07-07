using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class EventStockflowEnd : OccurrentStockflowEnd<EventId>
{
    protected EventStockflowEnd()
    {
    }

    protected EventStockflowEnd(StockflowEndId id, StockflowId stockflowId, EventId eventId)
        : base(id, stockflowId, eventId)
    {
    }

    protected void UpdateEventEnd(StockflowId stockflowId, EventId eventId)
        => UpdateOccurrentEnd(stockflowId, eventId);
}
