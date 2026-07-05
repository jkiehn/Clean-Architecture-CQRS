namespace CleanArchitectureCQRS.Domain.Exceptions;

public class TypificationInvalidException : Exception
{
    public TypificationInvalidException() : base("Typification is invalid.")
    {
    }
}
