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

    private static readonly EntityActionDefinition CreateCustomerPaymentAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("when", "When", EntityFieldKind.DateTime, Required: true, Placeholder: "N, T, +1d, or 2026-07-03T14:30:00+00:00", DefaultValue: "N"),
            new EntityFieldDefinition("endWhen", "End When", EntityFieldKind.DateTime, Placeholder: "Optional"),
            new EntityFieldDefinition("amount", "Amount", EntityFieldKind.Number, Required: true, Placeholder: "125.50"),
            new EntityFieldDefinition("customer", "Customer", Required: true, Placeholder: "Customer email, name, or id", Lookup: new EntityLookupDefinition("customers", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("cash", "Cash", Required: true, Placeholder: "Cash name or id", Lookup: new EntityLookupDefinition("cash"))
        });

    private static readonly EntityActionDefinition EditCustomerPaymentAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("when", "When", EntityFieldKind.DateTime, Required: true, Placeholder: "N, T, +1d, or 2026-07-03T14:30:00+00:00"),
            new EntityFieldDefinition("endWhen", "End When", EntityFieldKind.DateTime, Placeholder: "Optional"),
            new EntityFieldDefinition("amount", "Amount", EntityFieldKind.Number, Required: true, Placeholder: "125.50"),
            new EntityFieldDefinition("customer", "Customer", Required: true, Placeholder: "Customer email, name, or id", Lookup: new EntityLookupDefinition("customers", ValueSource: EntityLookupValueSource.Subtitle)),
            new EntityFieldDefinition("cash", "Cash", Required: true, Placeholder: "Cash name or id", Lookup: new EntityLookupDefinition("cash"))
        });

    private static readonly EntityActionDefinition CreatePaysForAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("sale", "Sale", Required: true, Placeholder: "Sale id", Lookup: new EntityLookupDefinition("sales")),
            new EntityFieldDefinition("customerPayment", "Customer Payment", Required: true, Placeholder: "Customer payment id", Lookup: new EntityLookupDefinition("customer-payments"))
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
            "Inventory",
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
            "cash",
            "Sales",
            "Cash",
            "Cash",
            "Create, inspect, update, and remove cash resources.",
            "bi-cash-stack",
            "Search cash resources by name",
            "No cash resources found yet. Create the first cash resource from the panel on the right.",
            16,
            CreateResourceSubtypeAction,
            EditResourceSubtypeAction,
            "Delete"),
        new EntityDescriptor(
            "customers",
            "Sales",
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
            "Procurement",
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
            "Human Resources",
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
            "Sales",
            "Sale",
            "Sales",
            "Create, inspect, update, and remove sales events.",
            "bi-currency-dollar",
            "Search sales by employee, customer, or amount. Use the Payments section for cash sale commands.",
            "No sales found yet. Create the first sale from the panel on the right.",
            28,
            CreateSaleAction,
            EditSaleAction,
            "Delete"),
        new EntityDescriptor(
            "customer-payments",
            "Sales",
            "Customer Payment",
            "Customer Payments",
            "Create, inspect, update, and remove customer payment events.",
            "bi-wallet2",
            "Search customer payments by customer, cash, or amount",
            "No customer payments found yet. Create the first payment from the panel on the right.",
            28,
            CreateCustomerPaymentAction,
            EditCustomerPaymentAction,
            "Delete"),
        new EntityDescriptor(
            "pays-for",
            "Sales",
            "Pays For",
            "Pays For",
            "Create, inspect, and remove links between sales and customer payments.",
            "bi-link-45deg",
            "Search pays-for links by customer or identifier",
            "No pays-for links found yet. Create the first link from the panel on the right.",
            28,
            CreatePaysForAction,
            null,
            "Delete"),
        new EntityDescriptor(
            "sales-orders",
            "Sales",
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
            "Procurement",
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
            "Procurement",
            "Prepaid IT Report",
            "Prepaid IT Report",
            "Review monthly prepaid IT expenses per department across all contracts.",
            "bi-bar-chart-line",
            "Open the prepaid IT expense report",
            "The prepaid IT expense report is ready to open.",
            32),
        new EntityDescriptor(
            "agents",
            "REA Base",
            "Agent",
            "Agents",
            "Read-only abstraction view across agent types.",
            "bi-diagram-nav-menu",
            "Search agents by name or email",
            "No agents found yet.",
            30)
    };
}
