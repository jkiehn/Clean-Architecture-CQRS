using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record EventId
{
    public Guid Value { get; }

    public EventId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new EventInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(EventId id)
        => id.Value;

    public static implicit operator EventId(Guid id)
        => new(id);
}
