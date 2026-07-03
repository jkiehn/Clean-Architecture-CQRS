using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class StockflowEndTest
{
    [Fact]
    public void UpdateEventEnd_Changes_StockflowId_And_EventId()
    {
        var eventEnd = new TestEventStockflowEnd(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var updatedStockflowId = new StockflowId(Guid.NewGuid());
        var updatedEventId = new EventId(Guid.NewGuid());

        eventEnd.Reassign(updatedStockflowId, updatedEventId);

        GetFieldValue<StockflowId>(eventEnd, "_stockflowId").ShouldBe(updatedStockflowId);
        GetFieldValue<EventId>(eventEnd, "_eventId").ShouldBe(updatedEventId);
    }

    [Fact]
    public void UpdateResourceEnd_Changes_StockflowId_And_ResourceId()
    {
        var resourceEnd = new TestResourceStockflowEnd(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var updatedStockflowId = new StockflowId(Guid.NewGuid());
        var updatedResourceId = new ResourceId(Guid.NewGuid());

        resourceEnd.Reassign(updatedStockflowId, updatedResourceId);

        GetFieldValue<StockflowId>(resourceEnd, "_stockflowId").ShouldBe(updatedStockflowId);
        GetFieldValue<ResourceId>(resourceEnd, "_resourceId").ShouldBe(updatedResourceId);
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

    private sealed class TestEventStockflowEnd : EventStockflowEnd
    {
        public TestEventStockflowEnd(Guid id, Guid stockflowId, Guid eventId)
            : base(id, stockflowId, eventId)
        {
        }

        public void Reassign(StockflowId stockflowId, EventId eventId)
            => UpdateEventEnd(stockflowId, eventId);
    }

    private sealed class TestResourceStockflowEnd : ResourceStockflowEnd
    {
        public TestResourceStockflowEnd(Guid id, Guid stockflowId, Guid resourceId)
            : base(id, stockflowId, resourceId)
        {
        }

        public void Reassign(StockflowId stockflowId, ResourceId resourceId)
            => UpdateResourceEnd(stockflowId, resourceId);
    }
}
