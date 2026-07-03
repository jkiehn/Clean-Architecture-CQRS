using CleanArchitectureCQRS.Domain.Exceptions;
using CleanArchitectureCQRS.Domain.ValueObjects;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Domain;

public class ResourceValueObjectsTests
{
    [Fact]
    public void ResourceId_Should_Throw_For_Empty_Guid()
    {
        var exception = Record.Exception(() => new ResourceId(Guid.Empty));

        exception.ShouldBeOfType<ResourceInvalidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void ResourceName_Should_Throw_For_Invalid_Value(string value)
    {
        var exception = Record.Exception(() => new ResourceName(value));

        exception.ShouldBeOfType<ResourceInvalidException>();
    }
}
