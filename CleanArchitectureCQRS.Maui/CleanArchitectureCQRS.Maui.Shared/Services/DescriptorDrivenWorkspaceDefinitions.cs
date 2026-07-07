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

    private static readonly EntityActionDefinition CreateSaleAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("when", "When", EntityFieldKind.DateTime, Required: true, Placeholder: "N, T, +1d, or 2026-07-03T14:30:00+00:00"),
            new EntityFieldDefinition("endWhen", "End When", EntityFieldKind.DateTime, Placeholder: "N, T, +1d, or 2026-07-03T15:00:00+00:00"),
            new EntityFieldDefinition("employee", "Employee", Required: true, Placeholder: "Employee email, name, id, or SSN", Lookup: new EntityLookupDefinition("employees", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("customer", "Customer", Required: true, Placeholder: "Customer email, name, or id", Lookup: new EntityLookupDefinition("customers", ValueSource: EntityLookupValueSource.Subtitle))
        });

    private static readonly EntityActionDefinition EditSaleAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("when", "When", EntityFieldKind.DateTime, Required: true, Placeholder: "N, T, +1d, or 2026-07-03T14:30:00+00:00"),
            new EntityFieldDefinition("endWhen", "End When", EntityFieldKind.DateTime, Placeholder: "N, T, +1d, or 2026-07-03T15:00:00+00:00"),
            new EntityFieldDefinition("employee", "Employee", Required: true, Placeholder: "Employee email, name, id, or SSN", Lookup: new EntityLookupDefinition("employees", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("customer", "Customer", Required: true, Placeholder: "Customer email, name, or id", Lookup: new EntityLookupDefinition("customers", ValueSource: EntityLookupValueSource.Subtitle))
        });

    private static readonly EntityActionDefinition CreateSalesOrderAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("when", "When", EntityFieldKind.DateTime, Required: true, Placeholder: "N, T, +1d, or 2026-07-03T14:30:00+00:00"),
            new EntityFieldDefinition("endWhen", "End When", EntityFieldKind.DateTime, Placeholder: "N, T, +1d, or 2026-07-03T15:00:00+00:00"),
            new EntityFieldDefinition("employee", "Employee", Required: true, Placeholder: "Employee email, name, id, or SSN", Lookup: new EntityLookupDefinition("employees", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("customer", "Customer", Required: true, Placeholder: "Customer email, name, or id", Lookup: new EntityLookupDefinition("customers", ValueSource: EntityLookupValueSource.Subtitle))
        });

    private static readonly EntityActionDefinition EditSalesOrderAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("when", "When", EntityFieldKind.DateTime, Required: true, Placeholder: "N, T, +1d, or 2026-07-03T14:30:00+00:00"),
            new EntityFieldDefinition("endWhen", "End When", EntityFieldKind.DateTime, Placeholder: "N, T, +1d, or 2026-07-03T15:00:00+00:00"),
            new EntityFieldDefinition("employee", "Employee", Required: true, Placeholder: "Employee email, name, id, or SSN", Lookup: new EntityLookupDefinition("employees", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("customer", "Customer", Required: true, Placeholder: "Customer email, name, or id", Lookup: new EntityLookupDefinition("customers", ValueSource: EntityLookupValueSource.Subtitle))
        });

    private static readonly EntityActionDefinition CreateItContractAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("serviceName", "Service Name", Required: true, Placeholder: "Microsoft 365 E5"),
            new EntityFieldDefinition("departmentCode", "Department Code", Required: true, Placeholder: "FIN-001"),
            new EntityFieldDefinition("startDate", "Start Date", EntityFieldKind.DateTime, Required: true, Placeholder: "2026-01-15"),
            new EntityFieldDefinition("endDate", "End Date", EntityFieldKind.DateTime, Required: true, Placeholder: "2027-01-14"),
            new EntityFieldDefinition("prepaidAmount", "Prepaid Amount", EntityFieldKind.Number, Required: true, Placeholder: "36500.00"),
            new EntityFieldDefinition("responsibleEmployee", "Responsible Employee", Required: true, Placeholder: "Employee email, name, id, or SSN", Lookup: new EntityLookupDefinition("employees", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("vendor", "Vendor", Required: true, Placeholder: "Vendor email, name, or id", Lookup: new EntityLookupDefinition("vendors", ValueSource: EntityLookupValueSource.Subtitle))
        });

    private static readonly EntityActionDefinition EditItContractAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("serviceName", "Service Name", Required: true, Placeholder: "Microsoft 365 E5"),
            new EntityFieldDefinition("departmentCode", "Department Code", Required: true, Placeholder: "FIN-001"),
            new EntityFieldDefinition("startDate", "Start Date", EntityFieldKind.DateTime, Required: true, Placeholder: "2026-01-15"),
            new EntityFieldDefinition("endDate", "End Date", EntityFieldKind.DateTime, Required: true, Placeholder: "2027-01-14"),
            new EntityFieldDefinition("prepaidAmount", "Prepaid Amount", EntityFieldKind.Number, Required: true, Placeholder: "36500.00"),
            new EntityFieldDefinition("responsibleEmployee", "Responsible Employee", Required: true, Placeholder: "Employee email, name, id, or SSN", Lookup: new EntityLookupDefinition("employees", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("vendor", "Vendor", Required: true, Placeholder: "Vendor email, name, or id", Lookup: new EntityLookupDefinition("vendors", ValueSource: EntityLookupValueSource.Subtitle))
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
            "sales",
            "Sale",
            "Sales",
            "Create, inspect, update, and remove sales events.",
            "bi-currency-dollar",
            "Search sales by employee, customer, or amount",
            "No sales found yet. Create the first sale from the panel on the right.",
            28,
            CreateSaleAction,
            EditSaleAction,
            "Delete"),
        new EntityDescriptor(
            "sales-orders",
            "Sales Order",
            "Sales Orders",
            "Create, inspect, update, and remove sales order commitments.",
            "bi-receipt-cutoff",
            "Search sales orders by employee, customer, or amount",
            "No sales orders found yet. Create the first sales order from the panel on the right.",
            29,
            CreateSalesOrderAction,
            EditSalesOrderAction,
            "Delete"),
        new EntityDescriptor(
            "it-contracts",
            "IT Contract",
            "IT Contracts",
            "Create, inspect, update, and remove prepaid IT contracts.",
            "bi-hdd-network",
            "Search IT contracts by service, vendor, department, employee, or amount",
            "No IT contracts found yet. Create the first prepaid IT contract from the panel on the right.",
            31,
            CreateItContractAction,
            EditItContractAction,
            "Delete"),
        new EntityDescriptor(
            "prepaid-it-report",
            "Prepaid IT Report",
            "Prepaid IT Report",
            "Review monthly prepaid IT expenses per department across all contracts.",
            "bi-bar-chart-line",
            "Open the prepaid IT expense report",
            "The prepaid IT expense report is ready to open.",
            32),
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
