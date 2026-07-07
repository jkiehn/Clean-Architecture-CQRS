namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class PaysForReadModel
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid CustomerPaymentId { get; set; }
}
