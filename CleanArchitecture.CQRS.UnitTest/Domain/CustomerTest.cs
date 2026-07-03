using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class CustomerTest
{
    [Fact]
    public void Customer_Should_Inherit_From_ExternalAgent()
    {
        var customer = new Customer(Guid.NewGuid(), "Alice Agent", "alice@example.com");

        customer.ShouldBeAssignableTo<ExternalAgent>();
    }

    [Fact]
    public void UpdateDetails_Changes_Name_And_Email()
    {
        var customer = new Customer(Guid.NewGuid(), "Alice Agent", "alice@example.com");

        customer.UpdateDetails("Alice Customer", "customer@example.com");

        GetFieldValue<AgentName>(customer, "_name").Value.ShouldBe("Alice Customer");
        GetFieldValue<AgentEmail>(customer, "_email").Value.ShouldBe("customer@example.com");
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
