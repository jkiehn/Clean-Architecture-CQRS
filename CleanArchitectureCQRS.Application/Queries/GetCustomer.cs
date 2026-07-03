using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;

namespace CleanArchitectureCQRS.Application.Queries;

public class GetCustomer : IQuery<CustomerDto>
{
    public Guid Id { get; set; }
}
