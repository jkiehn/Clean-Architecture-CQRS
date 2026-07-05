using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record OccurrentTypeId
{
    public Guid Value { get; }

    public OccurrentTypeId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new OccurrentTypeInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(OccurrentTypeId id)
        => id.Value;

    public static implicit operator OccurrentTypeId(Guid id)
        => new(id);
}
