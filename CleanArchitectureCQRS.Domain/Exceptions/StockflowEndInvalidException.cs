namespace CleanArchitectureCQRS.Domain.Exceptions;

public class StockflowEndInvalidException : Exception
{
    public StockflowEndInvalidException() : base("Stockflow end is invalid.")
    {
    }
}
