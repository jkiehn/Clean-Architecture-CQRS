using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record OccurrentTypeName
{
    public string Value { get; }

    public OccurrentTypeName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new OccurrentTypeInvalidException();
        }

        Value = value.Trim();
    }

    public static implicit operator string(OccurrentTypeName name)
        => name.Value;

    public static implicit operator OccurrentTypeName(string name)
        => new(name);
}
