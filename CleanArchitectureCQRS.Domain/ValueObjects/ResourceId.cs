using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record ResourceId
{
    public Guid Value { get; }

    public ResourceId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ResourceInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(ResourceId id)
        => id.Value;

    public static implicit operator ResourceId(Guid id)
        => new(id);
}
