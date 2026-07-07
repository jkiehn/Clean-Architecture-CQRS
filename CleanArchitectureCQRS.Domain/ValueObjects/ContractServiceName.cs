using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record ContractServiceName
{
    public string Value { get; }

    public ContractServiceName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ContractInvalidException();
        }

        Value = value.Trim();
    }

    public static implicit operator string(ContractServiceName name)
        => name.Value;

    public static implicit operator ContractServiceName(string name)
        => new(name);
}
