namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class SalesOrderLineReadModel
{
    public Guid Id { get; set; }
    public Guid OccurrentEndId { get; set; }
    public Guid ResourceEndId { get; set; }
    public Guid SalesOrderId { get; set; }
    public Guid ItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
}
