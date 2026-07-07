namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class CustomerPaymentCashFlowReadModel
{
    public Guid Id { get; set; }
    public Guid OccurrentEndId { get; set; }
    public Guid ResourceEndId { get; set; }
    public Guid CustomerPaymentId { get; set; }
    public Guid CashResourceId { get; set; }
}
