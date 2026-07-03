using CleanArchitectureCQRS.Application.Commands;
using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureCQRS.Api.Controllers;

public abstract class AgentSubtypeControllerBase<TGetQuery, TSearchQuery, TCreateCommand, TUpdateCommand, TRemoveCommand> : BaseController
    where TGetQuery : class, IQuery<AgentSubtypeDto>
    where TSearchQuery : class, IQuery<IEnumerable<AgentSubtypeDto>>
    where TCreateCommand : class, ICreateAgentSubtypeCommand
    where TUpdateCommand : class, IUpdateAgentSubtypeCommand
    where TRemoveCommand : class, IRemoveAgentSubtypeCommand
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly Func<TUpdateCommand, Guid, TUpdateCommand> _withRouteId;
    private readonly Func<Guid, TRemoveCommand> _removeCommandFactory;

    protected AgentSubtypeControllerBase(
        ICommandDispatcher commandDispatcher,
        IQueryDispatcher queryDispatcher,
        Func<TUpdateCommand, Guid, TUpdateCommand> withRouteId,
        Func<Guid, TRemoveCommand> removeCommandFactory)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
        _withRouteId = withRouteId;
        _removeCommandFactory = removeCommandFactory;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AgentSubtypeDto>> Get([FromRoute] TGetQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AgentSubtypeDto>>> Get([FromQuery] TSearchQuery query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TCreateCommand command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] TUpdateCommand command)
    {
        await _commandDispatcher.DispatchAsync(_withRouteId(command, id));
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _commandDispatcher.DispatchAsync(_removeCommandFactory(id));
        return Ok();
    }
}
