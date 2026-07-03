namespace CleanArchitectureCQRS.Domain.Exceptions;

public class EventInvalidException : Exception
{
    public EventInvalidException() : base("Event is invalid.")
    {
    }
}
