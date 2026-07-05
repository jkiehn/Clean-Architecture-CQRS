using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class TypificationTest
{
    [Fact]
    public void UpdateEnds_Changes_Occurrent_And_OccurrentType()
    {
        var updatedEventId = new EventId(Guid.NewGuid());
        var updatedOccurrentTypeId = new OccurrentTypeId(Guid.NewGuid());
        var typification = new TestEventTypification(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        typification.Reassign(updatedEventId, updatedOccurrentTypeId);

        GetFieldValue<EventId>(typification, "_occurrentId").ShouldBe(updatedEventId);
        GetFieldValue<OccurrentTypeId>(typification, "_occurrentTypeId").ShouldBe(updatedOccurrentTypeId);
    }

    [Fact]
    public void CommitmentTypification_Stores_Commitment_End_Ids()
    {
        var commitmentId = new CommitmentId(Guid.NewGuid());
        var occurrentTypeId = new OccurrentTypeId(Guid.NewGuid());
        var typification = new CommitmentTypification(Guid.NewGuid(), commitmentId, occurrentTypeId);

        GetFieldValue<CommitmentId>(typification, "_occurrentId").ShouldBe(commitmentId);
        GetFieldValue<OccurrentTypeId>(typification, "_occurrentTypeId").ShouldBe(occurrentTypeId);
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

    private sealed class TestEventTypification : Typification<EventId>
    {
        public TestEventTypification(Guid id, Guid occurrentId, Guid occurrentTypeId)
            : base(id, occurrentId, occurrentTypeId)
        {
        }

        public void Reassign(EventId occurrentId, OccurrentTypeId occurrentTypeId)
            => UpdateEnds(occurrentId, occurrentTypeId);
    }
}
