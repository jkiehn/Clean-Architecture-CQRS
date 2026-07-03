namespace CleanArchitectureCQRS.Maui.Shared.Services;

public abstract class AgentSubtypeWorkspaceServiceBase : EntityWorkspaceServiceBase
{
    private static readonly EntityActionDefinition CreateAction = new(
        "create",
        "Create",
        "btn btn-primary action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Agent name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "agent@example.com")
        });

    private static readonly EntityActionDefinition EditAction = new(
        "edit",
        "Save changes",
        "btn btn-dark action-btn",
        new[]
        {
            new EntityFieldDefinition("name", "Name", Required: true, Placeholder: "Agent name"),
            new EntityFieldDefinition("email", "Email", Required: true, Placeholder: "agent@example.com")
        });

    private readonly AgentSubtypeApiServiceBase _service;
    private readonly string _agentType;

    protected AgentSubtypeWorkspaceServiceBase(AgentSubtypeApiServiceBase service, EntityDescriptor descriptor, string agentType)
    {
        _service = service;
        _agentType = agentType;
        Descriptor = descriptor with
        {
            CreateAction = descriptor.CreateAction ?? CreateAction,
            EditAction = descriptor.EditAction ?? EditAction,
            DeleteLabel = descriptor.DeleteLabel ?? "Delete"
        };
    }

    public override EntityDescriptor Descriptor { get; }

    public override async Task<IReadOnlyList<EntityListItem>> SearchAsync(string searchPhrase = "")
    {
        var agents = await _service.GetAllAsync(searchPhrase);

        return agents
            .Select(agent => new EntityListItem(
                agent.Id,
                agent.Name,
                agent.Email,
                new[]
                {
                    new EntityProperty("Type", _agentType)
                }))
            .ToArray();
    }

    public override async Task<EntityDetail?> GetAsync(Guid id)
    {
        var agent = await _service.GetByIdAsync(id);

        if (agent is null)
        {
            return null;
        }

        return new EntityDetail(
            agent.Id,
            agent.Name,
            agent.Email,
            new[]
            {
                new EntityProperty("Email", agent.Email),
                new EntityProperty("Identifier", agent.Id.ToString()),
                new EntityProperty("Type", _agentType)
            },
            new Dictionary<string, object?>
            {
                ["name"] = agent.Name,
                ["email"] = agent.Email
            },
            Array.Empty<EntityCollectionSection>());
    }

    public override async Task<EntityOperationResult> CreateAsync(IReadOnlyDictionary<string, object?> values)
    {
        var id = Guid.NewGuid();
        await _service.CreateAsync(new CreateAgentSubtypeCommand(id, GetRequiredString(values, "name"), GetRequiredString(values, "email")));
        return new EntityOperationResult($"{Descriptor.DisplayName} created.", id);
    }

    public override async Task<EntityOperationResult> UpdateAsync(Guid id, IReadOnlyDictionary<string, object?> values)
    {
        await _service.UpdateAsync(new UpdateAgentSubtypeCommand(id, GetRequiredString(values, "name"), GetRequiredString(values, "email")));
        return new EntityOperationResult($"{Descriptor.DisplayName} updated.", id);
    }

    public override async Task<EntityOperationResult> DeleteAsync(Guid id)
    {
        await _service.DeleteAsync(id);
        return new EntityOperationResult($"{Descriptor.DisplayName} deleted.");
    }
}
