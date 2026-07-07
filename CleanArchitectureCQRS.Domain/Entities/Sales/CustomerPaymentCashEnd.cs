using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

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
