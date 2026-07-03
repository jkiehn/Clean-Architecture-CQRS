using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class SearchItems : IQuery<IEnumerable<ResourceSubtypeDto>>
{
    public string? SearchPhrase { get; set; }
}
