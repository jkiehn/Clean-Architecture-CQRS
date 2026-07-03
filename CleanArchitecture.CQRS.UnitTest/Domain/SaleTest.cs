using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class SaleTest
{
    [Fact]
    public void GetParticipations_Returns_Internal_And_External_Participations_For_Sale()
    {
        var sale = new Sale(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount: 125.50m);

        sale.GetInternalParticipation().ShouldBeOfType<SaleInternalParticipation>();
        sale.GetExternalParticipation().ShouldBeOfType<SaleExternalParticipation>();
    }

    [Fact]
    public void UpdateDetails_Changes_When_And_Participant_Ids()
    {
        var sale = new Sale(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount: 125.50m);
        var updatedWhen = new DateTimeOffset(2026, 7, 4, 12, 30, 0, TimeSpan.Zero);
        var updatedEmployeeId = new AgentId(Guid.NewGuid());
        var updatedCustomerId = new AgentId(Guid.NewGuid());

        sale.UpdateDetails(updatedWhen, updatedEmployeeId, updatedCustomerId);

        GetFieldValue<DateTimeOffset>(sale, "_when").ShouldBe(updatedWhen);
        GetFieldValue<AgentId>(sale, "_employeeId").ShouldBe(updatedEmployeeId);
        GetFieldValue<AgentId>(sale, "_customerId").ShouldBe(updatedCustomerId);
    }

    [Fact]
    public void AddSalesLine_Recalculates_Sale_Amount()
    {
        var sale = new Sale(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid());

        sale.AddSalesLine(Guid.NewGuid(), 10m, 2m);
        sale.AddSalesLine(Guid.NewGuid(), 5m, 3m);

        GetFieldValue<decimal?>(sale, "_amount").ShouldBe(35m);
        sale.GetSalesLines().Count.ShouldBe(2);
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
}
