using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class GetVendor : IQuery<AgentSubtypeDto>
{
    public Guid Id { get; set; }
}
