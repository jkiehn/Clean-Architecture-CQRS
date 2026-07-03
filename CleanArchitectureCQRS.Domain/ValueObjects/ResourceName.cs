using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record ResourceName
{
    public string Value { get; }

    public ResourceName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ResourceInvalidException();
        }

        Value = value.Trim();
    }

    public static implicit operator string(ResourceName name)
        => name.Value;

    public static implicit operator ResourceName(string name)
        => new(name);
}
