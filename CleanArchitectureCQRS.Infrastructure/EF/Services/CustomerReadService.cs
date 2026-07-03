using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;

namespace CleanArchitectureCQRS.Infrastructure.EF.Services;

internal sealed class CustomerReadService : AgentSubtypeReadService<CleanArchitectureCQRS.Infrastructure.EF.Models.CustomerReadModel>, ICustomerReadService
{
    public CustomerReadService(ReadDbContext context)
        : base(context.Customers)
    {
    }
}
