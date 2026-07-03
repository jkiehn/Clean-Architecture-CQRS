using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record AgentId
{
    public Guid Value { get; }

    public AgentId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new AgentInvalidException();
        }

        Value = value;
    }

    public static implicit operator Guid(AgentId id)
        => id.Value;

    public static implicit operator AgentId(Guid id)
        => new(id);
}
