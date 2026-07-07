using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class OccurrentStockflowEnd<TOccurrentId> : AggregateRoot<StockflowEndId>
{
    private StockflowId _stockflowId = default!;
    private TOccurrentId _occurrentId = default!;

    protected OccurrentStockflowEnd()
    {
    }

    protected OccurrentStockflowEnd(StockflowEndId id, StockflowId stockflowId, TOccurrentId occurrentId)
    {
        Id = id;
        UpdateOccurrentEnd(stockflowId, occurrentId);
    }

    protected void UpdateOccurrentEnd(StockflowId stockflowId, TOccurrentId occurrentId)
    {
        _stockflowId = stockflowId;
        _occurrentId = occurrentId;
    }
}
