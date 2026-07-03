using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class EmployeeTest
{
    [Fact]
    public void UpdateDetails_Changes_Name_Email_And_SocialSecurityNumber()
    {
        var employee = new Employee(Guid.NewGuid(), "Alice Agent", "alice@example.com", "111-11-1111");

        employee.UpdateDetails("Alice Employee", "employee@example.com", "222-22-2222");

        GetFieldValue<AgentName>(employee, "_name").Value.ShouldBe("Alice Employee");
        GetFieldValue<AgentEmail>(employee, "_email").Value.ShouldBe("employee@example.com");
        GetFieldValue<string>(employee, "_socialSecurityNumber").ShouldBe("222-22-2222");
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
