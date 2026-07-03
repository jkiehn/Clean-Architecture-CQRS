using CleanArchitectureCQRS.Application.Commands;
using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureCQRS.Api.Controllers;

public class CustomerController : BaseController
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;

    public CustomerController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> Get([FromRoute] GetCustomer query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return OkOrNotFound(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> Get([FromQuery] SearchCustomers query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCustomer command)
    {
        await _commandDispatcher.DispatchAsync(command);
        return CreatedAtAction(nameof(Get), new { id = command.Id }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateCustomer command)
    {
        await _commandDispatcher.DispatchAsync(command with { Id = id });
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _commandDispatcher.DispatchAsync(new RemoveCustomer(id));
        return Ok();
    }
}
