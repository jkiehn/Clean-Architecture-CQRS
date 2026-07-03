using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Event : AggregateRoot<EventId>
{
    private DateTimeOffset _when;
    private DateTimeOffset? _endWhen;
    private decimal? _amount;

    protected Event()
    {
    }

    protected Event(EventId id, DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
    {
        Id = id;
        UpdateTemporalDetails(when, endWhen, amount);
    }

    protected void UpdateTemporalDetails(DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
    {
        if (endWhen is not null && endWhen < when)
        {
            throw new EventInvalidException();
        }

        _when = when;
        _endWhen = endWhen;
        _amount = amount;
    }
}
