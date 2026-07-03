namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class VendorWorkspaceService : AgentSubtypeWorkspaceServiceBase
{
    public VendorWorkspaceService(VendorService service)
        : base(
            service,
            new EntityDescriptor(
                "vendors",
                "Vendor",
                "Vendors",
                "Create, inspect, update, and remove vendor agents.",
                "bi-briefcase-nav-menu",
                "Search vendors by name or email",
                "No vendors found yet. Create the first vendor from the panel on the right.",
                25),
            "Vendor")
    {
    }
}
