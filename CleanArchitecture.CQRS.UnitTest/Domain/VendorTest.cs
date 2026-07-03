using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class VendorTest
{
    [Fact]
    public void Vendor_Should_Inherit_From_ExternalAgent()
    {
        var vendor = new Vendor(Guid.NewGuid(), "Northwind", "sales@northwind.example");

        vendor.ShouldBeAssignableTo<ExternalAgent>();
    }

    [Fact]
    public void UpdateDetails_Changes_Name_And_Email()
    {
        var vendor = new Vendor(Guid.NewGuid(), "Northwind", "sales@northwind.example");

        vendor.UpdateDetails("Contoso Supply", "contact@contoso.example");

        GetFieldValue<AgentName>(vendor, "_name").Value.ShouldBe("Contoso Supply");
        GetFieldValue<AgentEmail>(vendor, "_email").Value.ShouldBe("contact@contoso.example");
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
