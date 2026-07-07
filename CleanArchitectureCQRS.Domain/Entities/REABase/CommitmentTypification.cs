using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public class CommitmentTypification : Typification<CommitmentId>
{
    public CommitmentTypification()
    {
    }

    public CommitmentTypification(TypificationId id, CommitmentId occurrentId, OccurrentTypeId occurrentTypeId)
        : base(id, occurrentId, occurrentTypeId)
    {
    }
}
