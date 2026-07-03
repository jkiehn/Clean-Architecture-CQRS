namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class CustomerWorkspaceService : AgentSubtypeWorkspaceServiceBase
{
    public CustomerWorkspaceService(CustomerService service)
        : base(
            service,
            new EntityDescriptor(
                "customers",
                "Customer",
                "Customers",
                "Create, inspect, update, and remove customer agents.",
                "bi-people-nav-menu",
                "Search customers by name or email",
                "No customers found yet. Create the first customer from the panel on the right.",
                20),
            "Customer")
    {
    }
}
