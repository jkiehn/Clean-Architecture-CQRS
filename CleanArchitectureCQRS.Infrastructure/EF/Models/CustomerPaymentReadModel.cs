namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class CustomerPaymentReadModel : EventReadModelBase
{
    public Guid ExternalParticipationId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CashFlowId { get; set; }
    public Guid CashResourceId { get; set; }
}
