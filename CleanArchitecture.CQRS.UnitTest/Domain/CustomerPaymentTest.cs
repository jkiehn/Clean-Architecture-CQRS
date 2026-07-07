using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class CustomerPaymentTest
{
    [Fact]
    public void GetExternalParticipation_And_CashFlow_Returns_Payment_Relationships()
    {
        var payment = new CustomerPayment(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 7, 9, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid(),
            250m);

        payment.GetExternalParticipation().ShouldBeOfType<CustomerPaymentExternalParticipation>();
        payment.GetCashFlow().ShouldBeOfType<CustomerPaymentCashFlow>();
    }

    [Fact]
    public void UpdateDetails_Changes_When_Customer_Cash_And_Amount()
    {
        var payment = new CustomerPayment(
            Guid.NewGuid(),
            new DateTimeOffset(2026, 7, 7, 9, 0, 0, TimeSpan.Zero),
            Guid.NewGuid(),
            Guid.NewGuid(),
            250m);
        var updatedWhen = new DateTimeOffset(2026, 7, 8, 10, 30, 0, TimeSpan.Zero);
        var updatedCustomerId = new AgentId(Guid.NewGuid());
        var updatedCashId = new ResourceId(Guid.NewGuid());

        payment.UpdateDetails(updatedWhen, updatedCustomerId, updatedCashId, 125m);

        GetFieldValue<DateTimeOffset>(payment, "_when").ShouldBe(updatedWhen);
        GetFieldValue<AgentId>(payment, "_customerId").ShouldBe(updatedCustomerId);
        GetFieldValue<ResourceId>(payment, "_cashResourceId").ShouldBe(updatedCashId);
        GetFieldValue<decimal?>(payment, "_amount").ShouldBe(125m);
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
