using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class SalesLineResourceEnd : ResourceStockflowEnd
{
    public SalesLineResourceEnd()
    {
    }

    public SalesLineResourceEnd(StockflowEndId id, StockflowId stockflowId, ResourceId resourceId)
        : base(id, stockflowId, resourceId)
    {
    }
}
