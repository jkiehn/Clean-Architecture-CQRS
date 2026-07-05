namespace CleanArchitectureCQRS.Domain.Exceptions;

public class OccurrentTypeInvalidException : Exception
{
    public OccurrentTypeInvalidException() : base("Occurrent type is invalid.")
    {
    }
}
