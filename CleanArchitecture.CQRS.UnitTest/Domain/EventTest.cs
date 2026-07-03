using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class EventTest
{
    [Fact]
    public void UpdateSchedule_Changes_When_EndWhen_And_Amount()
    {
        var initialWhen = new DateTimeOffset(2026, 7, 3, 8, 0, 0, TimeSpan.Zero);
        var updatedWhen = initialWhen.AddHours(2);
        var updatedEndWhen = updatedWhen.AddHours(3);
        var calendarEvent = new TestEvent(Guid.NewGuid(), initialWhen);

        calendarEvent.Reschedule(updatedWhen, updatedEndWhen, 42.5m);

        GetFieldValue<DateTimeOffset>(calendarEvent, "_when").ShouldBe(updatedWhen);
        GetFieldValue<DateTimeOffset?>(calendarEvent, "_endWhen").ShouldBe(updatedEndWhen);
        GetFieldValue<decimal?>(calendarEvent, "_amount").ShouldBe(42.5m);
    }

    [Fact]
    public void Constructor_Throws_When_EndWhen_Is_Before_When()
    {
        var when = new DateTimeOffset(2026, 7, 3, 8, 0, 0, TimeSpan.Zero);

        Should.Throw<EventInvalidException>(() =>
            new TestEvent(Guid.NewGuid(), when, when.AddMinutes(-30), 12m));
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

    private sealed class TestEvent : Event
    {
        public TestEvent(Guid id, DateTimeOffset when, DateTimeOffset? endWhen = null, decimal? amount = null)
            : base(id, when, endWhen, amount)
        {
        }

        public void Reschedule(DateTimeOffset when, DateTimeOffset? endWhen, decimal? amount)
            => UpdateTemporalDetails(when, endWhen, amount);
    }
}
