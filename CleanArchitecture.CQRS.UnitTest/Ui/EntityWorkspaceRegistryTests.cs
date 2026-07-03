using CleanArchitectureCQRS.Maui.Shared.Services;
using Shouldly;

namespace CleanArchitecture.CQRS.UnitTest.Ui;

public class EntityWorkspaceRegistryTests
{
    [Fact]
    public void Registry_Exposes_All_Entity_Workspaces_In_Order()
    {
        var services = new IEntityWorkspaceService[]
        {
            new StubWorkspaceService(new EntityDescriptor("agents", "Agent", "Agents", "Read-only agent view", "bi-diagram-nav-menu", "Search agents", "None", 30)),
            new StubWorkspaceService(new EntityDescriptor("sample-entities", "Sample Entity", "Sample Entities", "Sample entity view", "bi-list-nested-nav-menu", "Search sample entities", "None", 10)),
            new StubWorkspaceService(new EntityDescriptor("customers", "Customer", "Customers", "Customer view", "bi-people-nav-menu", "Search customers", "None", 20)),
            new StubWorkspaceService(new EntityDescriptor("vendors", "Vendor", "Vendors", "Vendor view", "bi-briefcase-nav-menu", "Search vendors", "None", 25))
        };

        var registry = new EntityWorkspaceRegistry(services);

        registry.Descriptors.Select(descriptor => descriptor.Key).ShouldBe(["sample-entities", "customers", "vendors", "agents"]);
        registry.Get("customers").Descriptor.PluralDisplayName.ShouldBe("Customers");
        registry.Get("vendors").Descriptor.PluralDisplayName.ShouldBe("Vendors");
    }

    private sealed class StubWorkspaceService : EntityWorkspaceServiceBase
    {
        public StubWorkspaceService(EntityDescriptor descriptor)
        {
            Descriptor = descriptor;
        }

        public override EntityDescriptor Descriptor { get; }

        public override Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "")
            => Task.FromResult<IReadOnlyList<EntityListItem>>(Array.Empty<EntityListItem>());

        public override Task<EntityDetail?> GetAsync(Guid id)
            => Task.FromResult<EntityDetail?>(null);
    }
}
