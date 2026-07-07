using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record DualityId
{
    public Guid Value { get; }

    public DualityId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DualityInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(DualityId id)
        => id.Value;

    public static implicit operator DualityId(Guid id)
        => new(id);
}
