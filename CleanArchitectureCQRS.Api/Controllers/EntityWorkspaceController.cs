using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureCQRS.Api.Controllers;

[ApiController]
[Route("api/entities/{entityKey}")]
public sealed class EntityWorkspaceController : ControllerBase
{
    private readonly IEntityWorkspaceBackendService _service;

    public EntityWorkspaceController(IEntityWorkspaceBackendService service)
    {
        _service = service;
    }

    [HttpGet]
    public Task<IReadOnlyList<EntityWorkspaceListItemDto>> Get([FromRoute] string entityKey, [FromQuery] string? searchPhrase = null)
        => _service.SearchAsync(entityKey, searchPhrase);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EntityWorkspaceDetailDto>> Get([FromRoute] string entityKey, [FromRoute] Guid id)
    {
        var result = await _service.GetAsync(entityKey, id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EntityWorkspaceOperationResultDto>> Post([FromRoute] string entityKey, [FromBody] EntityWorkspaceMutationRequest request)
    {
        var result = await _service.CreateAsync(entityKey, request.Values);
        return CreatedAtAction(nameof(Get), new { entityKey, id = result.EntityId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EntityWorkspaceOperationResultDto>> Put([FromRoute] string entityKey, [FromRoute] Guid id, [FromBody] EntityWorkspaceMutationRequest request)
    {
        return Ok(await _service.UpdateAsync(entityKey, id, request.Values));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<EntityWorkspaceOperationResultDto>> Delete([FromRoute] string entityKey, [FromRoute] Guid id)
    {
        return Ok(await _service.DeleteAsync(entityKey, id));
    }

    [HttpPost("{id:guid}/collections/{collectionKey}/actions/{actionKey}")]
    public async Task<ActionResult<EntityWorkspaceOperationResultDto>> ExecuteCollectionAction(
        [FromRoute] string entityKey,
        [FromRoute] Guid id,
        [FromRoute] string collectionKey,
        [FromRoute] string actionKey,
        [FromBody] EntityWorkspaceMutationRequest request)
    {
        return Ok(await _service.ExecuteCollectionActionAsync(entityKey, id, collectionKey, actionKey, request.Values));
    }

    [HttpPost("{id:guid}/collections/{collectionKey}/items/{itemKey}/actions/{actionKey}")]
    public async Task<ActionResult<EntityWorkspaceOperationResultDto>> ExecuteCollectionItemAction(
        [FromRoute] string entityKey,
        [FromRoute] Guid id,
        [FromRoute] string collectionKey,
        [FromRoute] string itemKey,
        [FromRoute] string actionKey)
    {
        return Ok(await _service.ExecuteCollectionItemActionAsync(entityKey, id, collectionKey, itemKey, actionKey));
    }
}
