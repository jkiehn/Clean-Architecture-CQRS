using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using CleanArchitectureCQRS.Infrastructure.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Services;

internal sealed class CustomerReadService : ICustomerReadService
{
    private readonly DbSet<CustomerReadModel> _customers;

    public CustomerReadService(ReadDbContext context)
    {
        _customers = context.Customers;
    }

    public Task<bool> ExistsByEmailAsync(string email, Guid? excludeCustomerId = null)
        => _customers.AnyAsync(customer =>
            customer.Email == email && (!excludeCustomerId.HasValue || customer.Id != excludeCustomerId.Value));
}
