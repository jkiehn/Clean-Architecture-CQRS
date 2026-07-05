using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class StockflowTest
{
    [Fact]
    public void UpdateRequiredEnds_Changes_OccurrentEndId_And_ResourceEndId()
    {
        var stockflow = new TestStockflow(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var updatedOccurrentEndId = new StockflowEndId(Guid.NewGuid());
        var updatedResourceEndId = new StockflowEndId(Guid.NewGuid());

        stockflow.Rewire(updatedOccurrentEndId, updatedResourceEndId);

        GetFieldValue<StockflowEndId>(stockflow, "_occurrentEndId").ShouldBe(updatedOccurrentEndId);
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
        public TestStockflow(Guid id, Guid occurrentEndId, Guid resourceEndId)
            : base(id, occurrentEndId, resourceEndId)
        {
        }

        public void Rewire(StockflowEndId occurrentEndId, StockflowEndId resourceEndId)
            => UpdateRequiredEnds(occurrentEndId, resourceEndId);
    }
}
