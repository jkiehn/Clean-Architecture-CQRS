using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class SalesOrderLineResourceEnd : ResourceStockflowEnd
{
    public SalesOrderLineResourceEnd()
    {
    }

    public SalesOrderLineResourceEnd(StockflowEndId id, StockflowId stockflowId, ResourceId resourceId)
        : base(id, stockflowId, resourceId)
    {
    }
}
