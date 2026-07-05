namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class SalesOrderReadModel : CommitmentReadModelBase
{
    public Guid InternalParticipationId { get; set; }
    public Guid EmployeeId { get; set; }
    public Guid ExternalParticipationId { get; set; }
    public Guid CustomerId { get; set; }
}
