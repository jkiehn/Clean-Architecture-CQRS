namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal abstract class ContinuantReadModelBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
