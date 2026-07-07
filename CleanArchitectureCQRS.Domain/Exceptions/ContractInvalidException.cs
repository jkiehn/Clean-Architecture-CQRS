namespace CleanArchitectureCQRS.Domain.Exceptions;

public class ContractInvalidException : Exception
{
    public ContractInvalidException() : base("Contract is invalid.")
    {
    }
}
