using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public class EventTypification : Typification<EventId>
{
    public EventTypification()
    {
    }

    public EventTypification(TypificationId id, EventId occurrentId, OccurrentTypeId occurrentTypeId)
        : base(id, occurrentId, occurrentTypeId)
    {
    }
}
