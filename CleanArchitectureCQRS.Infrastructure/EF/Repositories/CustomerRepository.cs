using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;

namespace CleanArchitectureCQRS.Infrastructure.EF.Repositories;

internal sealed class CustomerRepository : AgentRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(WriteDbContext writeDbContext)
        : base(writeDbContext, writeDbContext.Customers)
    {
    }
}
