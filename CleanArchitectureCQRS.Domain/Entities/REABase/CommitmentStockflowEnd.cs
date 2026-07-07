using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.REABase;

public abstract class CommitmentStockflowEnd : OccurrentStockflowEnd<CommitmentId>
{
    protected CommitmentStockflowEnd()
    {
    }

    protected CommitmentStockflowEnd(StockflowEndId id, StockflowId stockflowId, CommitmentId commitmentId)
        : base(id, stockflowId, commitmentId)
    {
    }

    protected void UpdateCommitmentEnd(StockflowId stockflowId, CommitmentId commitmentId)
        => UpdateOccurrentEnd(stockflowId, commitmentId);
}
