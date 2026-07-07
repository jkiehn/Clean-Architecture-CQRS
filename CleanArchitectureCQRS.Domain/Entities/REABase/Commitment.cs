using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class Commitment : Occurrent<CommitmentId>
{
    protected Commitment()
    {
    }

    protected Commitment(CommitmentId id, DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
        : base(id, when, endWhen, amount)
    {
    }

    protected new void UpdateTemporalDetails(DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
        => base.UpdateTemporalDetails(when, endWhen, amount);

    protected override Exception CreateInvalidException()
        => new CommitmentInvalidException();
}
