namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class CustomerService : AgentSubtypeApiServiceBase
{
    public CustomerService(HttpClient httpClient) : base(httpClient, "Customer")
    {
    }
}
