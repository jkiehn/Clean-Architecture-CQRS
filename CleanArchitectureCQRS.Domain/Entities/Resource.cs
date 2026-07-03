using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities;

public abstract class Resource : Continuant<ResourceId, ResourceName>
{
    protected Resource()
    {
    }

    protected Resource(ResourceId id, ResourceName name) : base(id, name)
    {
    }

    public void UpdateDetails(ResourceName name)
    {
        UpdateName(name);
    }
}
