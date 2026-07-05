using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Occurrent<TId> : AggregateRoot<TId>
{
    private DateTimeOffset _when;
    private DateTimeOffset? _endWhen;
    private decimal? _amount;

    protected Occurrent()
    {
    }

    protected Occurrent(TId id, DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
    {
        Id = id;
        UpdateTemporalDetails(when, endWhen, amount);
    }

    protected void UpdateTemporalDetails(DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
    {
        if (endWhen is not null && endWhen < when)
        {
            throw CreateInvalidException();
        }

        _when = when;
        _endWhen = endWhen;
        _amount = amount;
    }

    protected abstract Exception CreateInvalidException();
}
