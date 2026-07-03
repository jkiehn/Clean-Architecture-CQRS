namespace CleanArchitectureCQRS.Infrastructure.EF.Models;

internal abstract class EventReadModelBase
{
    public Guid Id { get; set; }
    public DateTimeOffset When { get; set; }
    public DateTimeOffset? EndWhen { get; set; }
    public decimal? Amount { get; set; }
}
