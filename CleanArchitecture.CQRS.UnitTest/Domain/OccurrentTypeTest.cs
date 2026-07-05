using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class OccurrentTypeTest
{
    [Fact]
    public void UpdateDetails_Changes_Name()
    {
        var occurrentType = new OccurrentType(Guid.NewGuid(), "Planned delivery");

        occurrentType.UpdateDetails("Promised shipment");

        GetFieldValue<OccurrentTypeName>(occurrentType, "_name").ShouldBe(new OccurrentTypeName("Promised shipment"));
    }

    [Fact]
    public void Constructor_Throws_When_Name_Is_Empty()
    {
        Should.Throw<OccurrentTypeInvalidException>(() =>
            new OccurrentType(Guid.NewGuid(), " "));
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
