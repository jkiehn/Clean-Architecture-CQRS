namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal sealed class ItContractReadModel : ContractReadModelBase
{
    public string ServiceName { get; set; } = string.Empty;
}
