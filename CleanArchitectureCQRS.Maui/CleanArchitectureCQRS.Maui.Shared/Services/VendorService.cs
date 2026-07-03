namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class VendorService : AgentSubtypeApiServiceBase
{
    public VendorService(HttpClient httpClient) : base(httpClient, "Vendor")
    {
    }
}
