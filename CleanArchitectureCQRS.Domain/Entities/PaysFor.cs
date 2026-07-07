using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

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
