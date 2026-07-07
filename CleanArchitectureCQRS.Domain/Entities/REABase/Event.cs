using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Event : Occurrent<EventId>
{
    protected Event()
    {
    }

    protected Event(EventId id, DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
    {
        Id = id;
        UpdateTemporalDetails(when, endWhen, amount);
    }

    protected new void UpdateTemporalDetails(DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
        => base.UpdateTemporalDetails(when, endWhen, amount);

    protected override Exception CreateInvalidException()
        => new EventInvalidException();
}
