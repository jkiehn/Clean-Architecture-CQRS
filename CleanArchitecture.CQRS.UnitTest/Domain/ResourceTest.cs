using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class ResourceTest
{
    [Fact]
    public void UpdateDetails_Changes_Name_Through_Continuant_Base()
    {
        var resource = new TestResource(Guid.NewGuid(), "Forklift");

        resource.UpdateDetails("Pallet Jack");

        GetFieldValue<ResourceName>(resource, "_name").Value.ShouldBe("Pallet Jack");
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

    private sealed class TestResource : Resource
    {
        public TestResource(Guid id, string name) : base(id, name)
        {
        }
    }
}
