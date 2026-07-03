using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class GetItem : IQuery<ResourceSubtypeDto>
{
    public Guid Id { get; set; }
}
