namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class ItemWorkspaceService : ResourceSubtypeWorkspaceServiceBase
{
    public ItemWorkspaceService(ItemService service)
        : base(
            service,
            new EntityDescriptor(
                "items",
                "Inventory",
                "Item",
                "Items",
                "Create, inspect, update, and remove resource items.",
                "bi-box-seam",
                "Search items by name",
                "No items found yet. Create the first item from the panel on the right.",
                15),
            "Item")
    {
    }
}
