namespace CleanArchitectureCQRS.Maui.Shared.Services;

public sealed class AgentWorkspaceService : EntityWorkspaceServiceBase
{
    private readonly AgentService _service;

    public AgentWorkspaceService(AgentService service)
    {
        _service = service;
    }

    public override EntityDescriptor Descriptor { get; } = new(
        "agents",
        "Agent",
        "Agents",
        "Read-only abstraction view across agent types.",
        "bi-diagram-nav-menu",
        "Search agents by name or email",
        "No agents found yet.",
        30);

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
                    new EntityProperty("Type", agent.AgentType)
                }))
            .ToArray();
    }

    public override async Task<EntityDetail?> GetAsync(Guid id)
    {
        var agent = (await _service.GetAllAsync()).FirstOrDefault(item => item.Id == id);

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
                new EntityProperty("Type", agent.AgentType),
                new EntityProperty("Identifier", agent.Id.ToString())
            },
            new Dictionary<string, object?>(),
            Array.Empty<EntityCollectionSection>());
    }
}
