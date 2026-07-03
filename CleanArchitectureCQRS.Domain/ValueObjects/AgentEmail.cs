using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record AgentEmail
{
    public string Value { get; }

    public AgentEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new AgentInvalidException();
        }

        var email = value.Trim();

        if (!email.Contains('@'))
        {
            throw new AgentInvalidException();
        }

        Value = email;
    }

    public static implicit operator string(AgentEmail email)
        => email.Value;

    public static implicit operator AgentEmail(string email)
        => new(email);
}
