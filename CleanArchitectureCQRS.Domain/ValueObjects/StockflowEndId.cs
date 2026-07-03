using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record StockflowEndId
{
    public Guid Value { get; }

    public StockflowEndId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new StockflowEndInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(StockflowEndId id)
        => id.Value;

    public static implicit operator StockflowEndId(Guid id)
        => new(id);
}
