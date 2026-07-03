using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureCQRS.Api.Controllers;

public class AgentController : BaseController
{
    private readonly IQueryDispatcher _queryDispatcher;

    public AgentController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AgentDto>>> Get([FromQuery] SearchAgents query)
    {
        var result = await _queryDispatcher.QueryAsync(query);
        return Ok(result);
    }
}
