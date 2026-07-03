namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal class SaleReadModel : EventReadModelBase
{
    public Guid InternalParticipationId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ExternalParticipationId { get; set; }
    public Guid CustomerId { get; set; }
}
