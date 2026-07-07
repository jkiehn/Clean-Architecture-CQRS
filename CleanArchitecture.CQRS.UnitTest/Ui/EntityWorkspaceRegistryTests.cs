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
            new StubWorkspaceService(new EntityDescriptor("agents", "REA Base", "Agent", "Agents", "Read-only agent view", "bi-diagram-nav-menu", "Search agents", "None", 30)),
            new StubWorkspaceService(new EntityDescriptor("sample-entities", "Samples", "Sample Entity", "Sample Entities", "Sample entity view", "bi-list-nested-nav-menu", "Search sample entities", "None", 10)),
            new StubWorkspaceService(new EntityDescriptor("items", "Inventory", "Item", "Items", "Item view", "bi-box-seam", "Search items", "None", 15)),
            new StubWorkspaceService(new EntityDescriptor("customers", "Sales", "Customer", "Customers", "Customer view", "bi-people-nav-menu", "Search customers", "None", 20)),
            new StubWorkspaceService(new EntityDescriptor("vendors", "Procurement", "Vendor", "Vendors", "Vendor view", "bi-briefcase-nav-menu", "Search vendors", "None", 25))
            ,new StubWorkspaceService(new EntityDescriptor("employees", "Human Resources", "Employee", "Employees", "Employee view", "bi-person-badge", "Search employees", "None", 27))
            ,new StubWorkspaceService(new EntityDescriptor("sales", "Sales", "Sale", "Sales", "Sale view", "bi-currency-dollar", "Search sales", "None", 28))
            ,new StubWorkspaceService(new EntityDescriptor("sales-orders", "Sales", "Sales Order", "Sales Orders", "Sales order view", "bi-receipt-cutoff", "Search sales orders", "None", 29))
            ,new StubWorkspaceService(new EntityDescriptor("it-contracts", "Procurement", "IT Contract", "IT Contracts", "IT contract view", "bi-hdd-network", "Search IT contracts", "None", 31))
            ,new StubWorkspaceService(new EntityDescriptor("prepaid-it-report", "Procurement", "Prepaid IT Report", "Prepaid IT Report", "Report view", "bi-bar-chart-line", "Open report", "None", 32))
        };

        var registry = new EntityWorkspaceRegistry(services);

        registry.Descriptors.Select(descriptor => descriptor.Key).ShouldBe(["sample-entities", "items", "customers", "vendors", "employees", "sales", "sales-orders", "agents", "it-contracts", "prepaid-it-report"]);
        registry.Get("items").Descriptor.PluralDisplayName.ShouldBe("Items");
        registry.Get("customers").Descriptor.PluralDisplayName.ShouldBe("Customers");
        registry.Get("vendors").Descriptor.PluralDisplayName.ShouldBe("Vendors");
        registry.Get("employees").Descriptor.PluralDisplayName.ShouldBe("Employees");
        registry.Get("sales").Descriptor.PluralDisplayName.ShouldBe("Sales");
        registry.Get("sales-orders").Descriptor.PluralDisplayName.ShouldBe("Sales Orders");
        registry.Get("it-contracts").Descriptor.PluralDisplayName.ShouldBe("IT Contracts");
        registry.Groups.Select(group => group.ProcessName).ShouldBe(["Samples", "Inventory", "Sales", "Procurement", "Human Resources", "REA Base"]);
        registry.Groups.Single(group => group.ProcessName == "Sales").Descriptors.Select(descriptor => descriptor.Key).ShouldBe(["customers", "sales", "sales-orders"]);
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
