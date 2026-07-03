using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class EventStockflowEnd : AggregateRoot<StockflowEndId>
{
    private StockflowId _stockflowId = default!;
    private EventId _eventId = default!;

    protected EventStockflowEnd()
    {
    }

    protected EventStockflowEnd(StockflowEndId id, StockflowId stockflowId, EventId eventId)
    {
        Id = id;
        UpdateEventEnd(stockflowId, eventId);
    }

    protected void UpdateEventEnd(StockflowId stockflowId, EventId eventId)
    {
        _stockflowId = stockflowId;
        _eventId = eventId;
    }
}
