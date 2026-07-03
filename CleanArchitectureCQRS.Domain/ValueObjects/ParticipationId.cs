using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record ParticipationId
{
    public Guid Value { get; }

    public ParticipationId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ParticipationInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(ParticipationId id)
        => id.Value;

    public static implicit operator ParticipationId(Guid id)
        => new(id);
}
