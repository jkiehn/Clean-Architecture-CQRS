using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class SalesOrderTest
{
    [Fact]
    public void GetParticipations_Returns_Internal_And_External_Participations_For_SalesOrder()
    {
        var salesOrder = new SalesOrder(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid(),
            amount: 125.50m);

        salesOrder.GetInternalParticipation().ShouldBeOfType<SalesOrderInternalParticipation>();
        salesOrder.GetExternalParticipation().ShouldBeOfType<SalesOrderExternalParticipation>();
    }

    [Fact]
    public void AddSalesOrderLine_Recalculates_SalesOrder_Amount()
    {
        var salesOrder = new SalesOrder(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 3, 10, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid());

        salesOrder.AddSalesOrderLine(Guid.NewGuid(), 10m, 2m);
        salesOrder.AddSalesOrderLine(Guid.NewGuid(), 5m, 3m);

        GetFieldValue<decimal?>(salesOrder, "_amount").ShouldBe(35m);
        salesOrder.GetSalesOrderLines().Count.ShouldBe(2);
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
