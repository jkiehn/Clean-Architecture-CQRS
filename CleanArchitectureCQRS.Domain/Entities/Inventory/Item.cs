using CleanArchitectureCQRS.Domain.ValueObjects;

namespace CleanArchitectureCQRS.Domain.Entities.Inventory;

public class Item : Resource
{
    public Item()
    {
    }

    public Item(ResourceId id, ResourceName name) : base(id, name)
    {
    }
}
