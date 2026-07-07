using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class CustomerPaymentCashEnd : ResourceStockflowEnd
{
    public CustomerPaymentCashEnd()
    {
    }

    public CustomerPaymentCashEnd(StockflowEndId id, StockflowId stockflowId, ResourceId resourceId)
        : base(id, stockflowId, resourceId)
    {
    }
}
