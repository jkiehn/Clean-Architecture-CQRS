using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record AgentName
{
    public string Value { get; }

    public AgentName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new AgentInvalidException();
        }

        Value = value.Trim();
    }

    public static implicit operator string(AgentName name)
        => name.Value;

    public static implicit operator AgentName(string name)
        => new(name);
}
