using CleanArchitectureCQRS.Application.Services;
using CleanArchitectureCQRS.Infrastructure.EF.Contexts;

namespace CleanArchitectureCQRS.Infrastructure.EF.Services;

internal sealed class VendorReadService : AgentSubtypeReadService<CleanArchitectureCQRS.Infrastructure.EF.Models.VendorReadModel>, IVendorReadService
{
    public VendorReadService(ReadDbContext context)
        : base(context.Vendors)
    {
    }
}
