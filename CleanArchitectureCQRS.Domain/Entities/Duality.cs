using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Duality : AggregateRoot<DualityId>
{
    private EventId _firstEventId = default!;
    private EventId _secondEventId = default!;

    protected Duality()
    {
    }

    protected Duality(DualityId id, EventId firstEventId, EventId secondEventId)
    {
        Id = id;
        UpdateEnds(firstEventId, secondEventId);
    }

    protected void UpdateEnds(EventId firstEventId, EventId secondEventId)
    {
        if (firstEventId == secondEventId)
        {
            throw new DualityInvalidException();
        }

        _firstEventId = firstEventId;
        _secondEventId = secondEventId;
    }
}
