using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class AgentValueObjectsTests
{
    [Fact]
    public void AgentId_Should_Throw_For_Empty_Guid()
    {
        var exception = Record.Exception(() => new AgentId(Guid.Empty));

        exception.ShouldBeOfType<AgentInvalidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void AgentName_Should_Throw_For_Invalid_Value(string value)
    {
        var exception = Record.Exception(() => new AgentName(value));

        exception.ShouldBeOfType<AgentInvalidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("missing-at-sign")]
    public void AgentEmail_Should_Throw_For_Invalid_Value(string value)
    {
        var exception = Record.Exception(() => new AgentEmail(value));

        exception.ShouldBeOfType<AgentInvalidException>();
    }

    [Fact]
    public void AgentEmail_Should_Trim_Valid_Value()
    {
        var email = new AgentEmail("  alice@example.com  ");

        email.Value.ShouldBe("alice@example.com");
    }
}
