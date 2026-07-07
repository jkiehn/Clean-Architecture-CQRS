using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Stockflow : AggregateRoot<StockflowId>
{
    private StockflowEndId _occurrentEndId = default!;
    private StockflowEndId _resourceEndId = default!;

    protected Stockflow()
    {
    }

    protected Stockflow(StockflowId id, StockflowEndId occurrentEndId, StockflowEndId resourceEndId)
    {
        Id = id;
        UpdateRequiredEnds(occurrentEndId, resourceEndId);
    }

    protected void UpdateRequiredEnds(StockflowEndId occurrentEndId, StockflowEndId resourceEndId)
    {
        _occurrentEndId = occurrentEndId;
        _resourceEndId = resourceEndId;
    }
}
