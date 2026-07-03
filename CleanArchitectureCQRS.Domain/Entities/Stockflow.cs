using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Stockflow : AggregateRoot<StockflowId>
{
    private StockflowEndId _eventEndId = default!;
    private StockflowEndId _resourceEndId = default!;

    protected Stockflow()
    {
    }

    protected Stockflow(StockflowId id, StockflowEndId eventEndId, StockflowEndId resourceEndId)
    {
        Id = id;
        UpdateRequiredEnds(eventEndId, resourceEndId);
    }

    protected void UpdateRequiredEnds(StockflowEndId eventEndId, StockflowEndId resourceEndId)
    {
        _eventEndId = eventEndId;
        _resourceEndId = resourceEndId;
    }
}
