using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Domain.ValueObjects;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureCQRS.Infrastructure.EF.Repositories;

internal sealed class CustomerRepository : ICustomerRepository
{
    private readonly DbSet<Customer> _customers;
    private readonly WriteDbContext _writeDbContext;

    public CustomerRepository(WriteDbContext writeDbContext)
    {
        _customers = writeDbContext.Customers;
        _writeDbContext = writeDbContext;
    }

    public Task<Customer?> GetAsync(AgentId id)
        => _customers.SingleOrDefaultAsync(customer => customer.Id == id);

    public async Task AddAsync(Customer customer)
    {
        await _customers.AddAsync(customer);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _customers.Update(customer);
        await _writeDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer customer)
    {
        _customers.Remove(customer);
        await _writeDbContext.SaveChangesAsync();
    }
}
