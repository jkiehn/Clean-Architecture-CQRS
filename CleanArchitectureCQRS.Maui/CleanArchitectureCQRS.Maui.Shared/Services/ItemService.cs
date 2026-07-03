namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class ItemService : ResourceSubtypeApiServiceBase
{
    public ItemService(HttpClient httpClient) : base(httpClient, "Item")
    {
    }
}
