using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Shared.Abstractions.Domains;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Typification<TOccurrentId> : AggregateRoot<TypificationId>
{
    private TOccurrentId _occurrentId = default!;
    private OccurrentTypeId _occurrentTypeId = default!;

    protected Typification()
    {
    }

    protected Typification(TypificationId id, TOccurrentId occurrentId, OccurrentTypeId occurrentTypeId)
    {
        Id = id;
        UpdateEnds(occurrentId, occurrentTypeId);
    }

    protected void UpdateEnds(TOccurrentId occurrentId, OccurrentTypeId occurrentTypeId)
    {
        _occurrentId = occurrentId;
        _occurrentTypeId = occurrentTypeId;
    }
}
