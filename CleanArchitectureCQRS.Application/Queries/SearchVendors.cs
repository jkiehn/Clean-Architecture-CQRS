using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class SearchVendors : IQuery<IEnumerable<AgentSubtypeDto>>
{
    public string? SearchPhrase { get; set; }
}
