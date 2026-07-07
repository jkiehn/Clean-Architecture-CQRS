using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public sealed class Cash : Resource
{
    public Cash()
    {
    }

    public Cash(ResourceId id, ResourceName name) : base(id, name)
    {
    }
}
