namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class CustomerWorkspaceService : EntityWorkspaceServiceBase
{
    private static readonly EntityActionDefinition CreateAction = new(
        "create",
        "Create customer",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Customer name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "customer@example.com")
        });

    private static readonly EntityActionDefinition EditAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Customer name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "customer@example.com")
        });

    private readonly CustomerService _service;

    public CustomerWorkspaceService(CustomerService service)
    {
        _service = service;
    }

    public override EntityDescriptor Descriptor { get; } = new(
        "customers",
        "Customer",
        "Customers",
        "Create, inspect, update, and remove customer agents.",
        "bi-people-nav-menu",
        "Search customers by name or email",
        "No customers found yet. Create the first customer from the panel on the right.",
        20,
        CreateAction,
        EditAction,
        "Delete");

    public override async Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "")
    {
        var customers = await _service.GetAllAsync(searchPhrase);

        return customers
            .Select(customer => new EntityListItem(
                customer.Id,
                customer.Name,
                customer.Email,
                new[]
                {
                    new EntityProperty("Type", "Customer")
                }))
            .ToArray();
    }

    public override async Task<EntityDetail?> GetAsync(Guid id)
    {
        var customer = await _service.GetByIdAsync(id);

        if (customer is null)
        {
            return null;
        }

        return new EntityDetail(
            customer.Id,
            customer.Name,
            customer.Email,
            new[]
            {
                new EntityProperty("Email", customer.Email),
                new EntityProperty("Identifier", customer.Id.ToString()),
                new EntityProperty("Type", "Customer")
            },
            new Dictionary<string, object?>
            {
                ["name"] = customer.Name,
                ["email"] = customer.Email
            },
            Array.Empty<EntityCollectionSection>());
    }

    public override async Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values)
    {
        var id = Guid.NewGuid();
        await _service.CreateAsync(new CreateCustomerCommand(id, GetRequiredString(values, "name"), GetRequiredString(values, "email")));
        return new EntityOperationResult("Customer created.", id);
    }

    public override async Task<EntityOperationResult> UpdateAsync(Guid id, IReadOnlyDictionary<string, object?> values)
    {
        await _service.UpdateAsync(new UpdateCustomerCommand(id, GetRequiredString(values, "name"), GetRequiredString(values, "email")));
        return new EntityOperationResult("Customer updated.", id);
    }

    public override async Task<EntityOperationResult> DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
        return new EntityOperationResult("Customer deleted.");
    }
}
