using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class SearchCustomers : IQuery<IEnumerable<AgentSubtypeDto>>
{
    public string? SearchPhrase { get; set; }
}
