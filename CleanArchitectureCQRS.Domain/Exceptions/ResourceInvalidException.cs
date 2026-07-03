namespace CleanArchitectureCQRS.Domain.Exceptions;

public class ResourceInvalidException : Exception
{
    public ResourceInvalidException() : base("Resource is invalid.")
    {
    }
}
