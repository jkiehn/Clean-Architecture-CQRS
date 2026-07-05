using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class SalesLineTest
{
    [Fact]
    public void Constructor_Creates_Occurrent_And_Resource_Ends_And_Computes_LineTotal()
    {
        var saleId = new EventId(Guid.NewGuid());
        var itemId = new ResourceId(Guid.NewGuid());
        var line = new SalesLine(Guid.NewGuid(), saleId, itemId, 12.5m, 3m);

        line.GetEventEnd().ShouldBeOfType<SalesLineEventEnd>();
        line.GetResourceEnd().ShouldBeOfType<SalesLineResourceEnd>();
        line.LineTotal.ShouldBe(37.5m);
    }

    [Fact]
    public void Constructor_Throws_When_Quantity_Is_Not_Positive()
    {
        Should.Throw<StockflowInvalidException>(() =>
            new SalesLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m, 0m));
    }
}
