namespace CleanArchitectureCQRS.Maui.Shared.Services;

public static class DescriptorDrivenWorkspaceDefinitions
{
    private static readonly EntityActionDefinition CreateAgentSubtypeAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Agent name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "agent@example.com")
        });

    private static readonly EntityActionDefinition EditAgentSubtypeAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Agent name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "agent@example.com")
        });

    private static readonly EntityActionDefinition CreateEmployeeAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Employee name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "employee@example.com"),
            new EntityFieldDefinition("socialSecurityNumber", "Social Security Number", Required: true, Placeholder: "123-45-6789")
        });

    private static readonly EntityActionDefinition EditEmployeeAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Employee name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "employee@example.com"),
            new EntityFieldDefinition("socialSecurityNumber", "Social Security Number", Required: true, Placeholder: "123-45-6789")
        });

    private static readonly EntityActionDefinition CreateResourceSubtypeAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Resource name")
        });

    private static readonly EntityActionDefinition EditResourceSubtypeAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Resource name")
        });

    public static IReadOnlyList<EntityDescriptor> All { get; } = new[]
    {
        new EntityDescriptor(
            "items",
            "Item",
            "Items",
            "Create, inspect, update, and remove resource items.",
            "bi-box-seam",
            "Search items by name",
            "No items found yet. Create the first item from the panel on the right.",
            15,
            CreateResourceSubtypeAction,
            EditResourceSubtypeAction,
            "Delete"),
        new EntityDescriptor(
            "customers",
            "Customer",
            "Customers",
            "Create, inspect, update, and remove customer agents.",
            "bi-people-nav-menu",
            "Search customers by name or email",
            "No customers found yet. Create the first customer from the panel on the right.",
            20,
            CreateAgentSubtypeAction,
            EditAgentSubtypeAction,
            "Delete"),
        new EntityDescriptor(
            "vendors",
            "Vendor",
            "Vendors",
            "Create, inspect, update, and remove vendor agents.",
            "bi-briefcase-nav-menu",
            "Search vendors by name or email",
            "No vendors found yet. Create the first vendor from the panel on the right.",
            25,
            CreateAgentSubtypeAction,
            EditAgentSubtypeAction,
            "Delete"),
        new EntityDescriptor(
            "employees",
            "Employee",
            "Employees",
            "Create, inspect, update, and remove employee agents.",
            "bi-person-badge",
            "Search employees by name, email, or social security number",
            "No employees found yet. Create the first employee from the panel on the right.",
            27,
            CreateEmployeeAction,
            EditEmployeeAction,
            "Delete"),
        new EntityDescriptor(
            "agents",
            "Agent",
            "Agents",
            "Read-only abstraction view across agent types.",
            "bi-diagram-nav-menu",
            "Search agents by name or email",
            "No agents found yet.",
            30)
    };
}
