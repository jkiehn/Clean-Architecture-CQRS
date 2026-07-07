using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class SalesOrderLineCommitmentEnd : CommitmentStockflowEnd
{
    public SalesOrderLineCommitmentEnd()
    {
    }

    public SalesOrderLineCommitmentEnd(StockflowEndId id, StockflowId stockflowId, CommitmentId commitmentId)
        : base(id, stockflowId, commitmentId)
    {
    }
}
