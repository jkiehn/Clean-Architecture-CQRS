using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class GetCustomer : IQuery<AgentSubtypeDto>
{
    public Guid Id { get; set; }
}
