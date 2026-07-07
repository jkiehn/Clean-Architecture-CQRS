using CleanArchitectureCQRS.Domain.Exceptions;

namespace CleanArchitectureCQRS.Domain.ValueObjects;

public record ContractDepartmentCode
{
    public string Value { get; }

    public ContractDepartmentCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ContractInvalidException();
        }

        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(ContractDepartmentCode code)
        => code.Value;

    public static implicit operator ContractDepartmentCode(string code)
        => new(code);
}
