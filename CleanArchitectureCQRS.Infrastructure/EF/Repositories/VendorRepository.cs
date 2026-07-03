using CleanArchitectureCQRS.Domain.Entities;
using CleanArchitectureCQRS.Domain.Repositories;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;

namespace CleanArchitectureCQRS.Infrastructure.EF.Repositories;

internal sealed class VendorRepository : AgentRepository<Vendor>, IVendorRepository
{
    public VendorRepository(WriteDbContext writeDbContext)
        : base(writeDbContext, writeDbContext.Vendors)
    {
    }
}
