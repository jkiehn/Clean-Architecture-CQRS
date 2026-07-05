namespace CleanArchitectureCQRS.Domain.Exceptions;

public class CommitmentInvalidException : Exception
{
    public CommitmentInvalidException() : base("Commitment is invalid.")
    {
    }
}
