namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal abstract class ContractReadModelBase : CommitmentReadModelBase
{
    public string DepartmentCode { get; set; } = string.Empty;
    public Guid InternalParticipationId { get; set; }
    public Guid ResponsibleEmployeeId { get; set; }
    public Guid ExternalParticipationId { get; set; }
    public Guid VendorId { get; set; }
}
