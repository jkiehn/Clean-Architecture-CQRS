using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class StockflowTest
{
    [Fact]
    public void UpdateRequiredEnds_Changes_EventEndId_And_ResourceEndId()
    {
        var stockflow = new TestStockflow(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var updatedEventEndId = new StockflowEndId(Guid.NewGuid());
        var updatedResourceEndId = new StockflowEndId(Guid.NewGuid());

        stockflow.Rewire(updatedEventEndId, updatedResourceEndId);

        GetFieldValue<StockflowEndId>(stockflow, "_eventEndId").ShouldBe(updatedEventEndId);
        GetFieldValue<StockflowEndId>(stockflow, "_resourceEndId").ShouldBe(updatedResourceEndId);
    }

    private static T GetFieldValue<T>(object instance, string fieldName)
    {
        var currentType = instance.GetType();
        System.Reflection.FieldInfo? field = null;

        while (currentType is not null && field is null)
        {
            field = currentType.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            currentType = currentType.BaseType;
        }

        field.ShouldNotBeNull();
        return (T)field.GetValue(instance)!;
    }

    private sealed class TestStockflow : Stockflow
    {
        public TestStockflow(Guid id, Guid eventEndId, Guid resourceEndId)
            : base(id, eventEndId, resourceEndId)
        {
        }

        public void Rewire(StockflowEndId eventEndId, StockflowEndId resourceEndId)
            => UpdateRequiredEnds(eventEndId, resourceEndId);
    }
}
