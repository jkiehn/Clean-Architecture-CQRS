using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Take : Stockflow
{
    protected Take()
    {
    }

    protected Take(StockflowId id, StockflowEndId eventEndId, StockflowEndId resourceEndId)
        : base(id, eventEndId, resourceEndId)
    {
    }
}
