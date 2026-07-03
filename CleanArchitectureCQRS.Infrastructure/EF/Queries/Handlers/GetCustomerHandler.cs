using CleanArchitectureCQRS.Application.DTOs;
using CleanArchitectureCQRS.Application.Queries;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Queries.Handlers;

internal sealed class GetCustomerHandler : IQueryHandler<GetCustomer, CustomerDto>
{
    private readonly DbSet<CustomerReadModel> _customers;

    public GetCustomerHandler(ReadDbContext context)
    {
        _customers = context.Customers;
    }

    public Task<CustomerDto?> HandleAsync(GetCustomer query)
        => _customers
            .Where(customer => customer.Id == query.Id)
            .Select(customer => customer.AsDto())
            .AsNoTracking()
            .SingleOrDefaultAsync();
}
