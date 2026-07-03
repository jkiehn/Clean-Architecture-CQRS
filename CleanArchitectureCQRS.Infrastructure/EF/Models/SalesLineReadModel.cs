namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class SalesLineReadModel
{
    public Guid Id { get; set; }
    public Guid EventEndId { get; set; }
    public Guid ResourceEndId { get; set; }
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
}
