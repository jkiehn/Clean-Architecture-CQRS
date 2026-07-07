using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class ResourceStockflowEnd : AggregateRoot<StockflowEndId>
{
    private StockflowId _stockflowId = default!;
    private ResourceId _resourceId = default!;

    protected ResourceStockflowEnd()
    {
    }

    protected ResourceStockflowEnd(StockflowEndId id, StockflowId stockflowId, ResourceId resourceId)
    {
        Id = id;
        UpdateResourceEnd(stockflowId, resourceId);
    }

    protected void UpdateResourceEnd(StockflowId stockflowId, ResourceId resourceId)
    {
        _stockflowId = stockflowId;
        _resourceId = resourceId;
    }
}
