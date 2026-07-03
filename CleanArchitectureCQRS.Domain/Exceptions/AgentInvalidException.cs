namespace CleanArchitectureCQRS.Domain.Exceptions;

public class AgentInvalidException : Exception
{
    public AgentInvalidException() : base("Agent is invalid.")
    {
    }
}
