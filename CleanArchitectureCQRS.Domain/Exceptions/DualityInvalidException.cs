namespace CleanArchitectureCQRS.Domain.Exceptions;

public class DualityInvalidException : Exception
{
    public DualityInvalidException() : base("Duality is invalid.")
    {
    }
}
