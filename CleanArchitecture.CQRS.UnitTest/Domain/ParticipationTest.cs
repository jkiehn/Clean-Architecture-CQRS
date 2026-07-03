using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class ParticipationTest
{
    [Fact]
    public void UpdateParticipants_Changes_AgentId_And_EventId()
    {
        var participation = new TestParticipation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        var updatedAgentId = new AgentId(Guid.NewGuid());
        var updatedEventId = new EventId(Guid.NewGuid());

        participation.Reassign(updatedAgentId, updatedEventId);

        GetFieldValue<AgentId>(participation, "_agentId").ShouldBe(updatedAgentId);
        GetFieldValue<EventId>(participation, "_eventId").ShouldBe(updatedEventId);
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

    private sealed class TestParticipation : Participation
    {
        public TestParticipation(Guid id, Guid agentId, Guid eventId)
            : base(id, agentId, eventId)
        {
        }

        public void Reassign(AgentId agentId, EventId eventId)
            => UpdateParticipants(agentId, eventId);
    }
}
