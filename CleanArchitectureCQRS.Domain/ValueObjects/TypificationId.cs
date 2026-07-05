using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record TypificationId
{
    public Guid Value { get; }

    public TypificationId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new TypificationInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(TypificationId id)
        => id.Value;

    public static implicit operator TypificationId(Guid id)
        => new(id);
}
