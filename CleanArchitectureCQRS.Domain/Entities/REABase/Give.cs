using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Give : Stockflow
{
    protected Give()
    {
    }

    protected Give(StockflowId id, StockflowEndId eventEndId, StockflowEndId resourceEndId)
        : base(id, eventEndId, resourceEndId)
    {
    }
}
