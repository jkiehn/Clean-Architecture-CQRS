using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Sales;

public sealed class Cash : Resource
{
    public Cash()
    {
    }

    public Cash(ResourceId id, ResourceName name) : base(id, name)
    {
    }
}
