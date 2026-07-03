namespace CleanArchitectureCQRS.Domain.Exceptions;

public class StockflowInvalidException : Exception
{
    public StockflowInvalidException() : base("Stockflow is invalid.")
    {
    }
}
