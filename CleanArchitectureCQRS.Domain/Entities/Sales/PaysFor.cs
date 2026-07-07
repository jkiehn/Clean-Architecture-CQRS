using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class PaysFor : Duality
{
    public PaysFor()
    {
    }

    public PaysFor(DualityId id, EventId saleId, EventId customerPaymentId)
        : base(id, saleId, customerPaymentId)
    {
    }
}
