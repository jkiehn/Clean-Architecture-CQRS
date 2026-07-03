namespace CleanArchitectureCQRS.Domain.Exceptions;

public class ParticipationInvalidException : Exception
{
    public ParticipationInvalidException() : base("Participation is invalid.")
    {
    }
}
