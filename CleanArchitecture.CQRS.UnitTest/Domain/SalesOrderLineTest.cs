using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class SalesOrderLineTest
{
    [Fact]
    public void Constructor_Creates_Commitment_And_Resource_Ends_And_Computes_LineTotal()
    {
        var salesOrderId = new CommitmentId(Guid.NewGuid());
        var itemId = new ResourceId(Guid.NewGuid());
        var line = new SalesOrderLine(Guid.NewGuid(), salesOrderId, itemId, 12.5m, 3m);

        line.GetCommitmentEnd().ShouldBeOfType<SalesOrderLineCommitmentEnd>();
        line.GetResourceEnd().ShouldBeOfType<SalesOrderLineResourceEnd>();
        line.LineTotal.ShouldBe(37.5m);
    }

    [Fact]
    public void Constructor_Throws_When_Quantity_Is_Not_Positive()
    {
        Should.Throw<StockflowInvalidException>(() =>
            new SalesOrderLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m, 0m));
    }
}
