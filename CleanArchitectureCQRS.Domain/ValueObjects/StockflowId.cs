using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record StockflowId
{
    public Guid Value { get; }

    public StockflowId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new StockflowInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(StockflowId id)
        => id.Value;

    public static implicit operator StockflowId(Guid id)
        => new(id);
}
