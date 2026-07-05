using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record CommitmentId
{
    public Guid Value { get; }

    public CommitmentId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new CommitmentInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(CommitmentId id)
        => id.Value;

    public static implicit operator CommitmentId(Guid id)
        => new(id);
}
